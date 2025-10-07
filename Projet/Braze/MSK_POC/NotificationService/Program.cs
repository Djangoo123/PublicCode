using Confluent.Kafka;
using System.Collections.Generic;
using System;
using System.Text.Json;

record SegmentEvent(string userId, string segmentId);

class Program
{
    static void Main()
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = "localhost:9092",
            GroupId = "notification-service",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();
        consumer.Subscribe("user-segments");

        Console.WriteLine("NotificationService en écoute...");

        var actions = new Dictionary<string, Action<string>>
        {
            ["premium_fr"] = userId => SendToBraze(userId, "braze_campaign_id_1"),
            ["churn_risk"] = userId => SendEmail(userId, "Relance"),
            ["mobile_active"] = userId => SendPush(userId, "Reviens dans l’app !")
        };

        while (true)
        {
            var cr = consumer.Consume();
            var evt = JsonSerializer.Deserialize<SegmentEvent>(cr.Message.Value);

            if (evt != null && actions.TryGetValue(evt.segmentId, out var action))
            {
                action(evt.userId);
            }
            else
            {
                Console.WriteLine($"⚠️ Aucun handler défini pour le segment {evt?.segmentId}");
            }
        }
    }

    static void SendToBraze(string userId, string campaignId)
    {
        Console.WriteLine($"📤 [Braze] User={userId} → Campaign={campaignId}");
    }

    static void SendEmail(string userId, string template)
    {
        Console.WriteLine($"📧 [Email] User={userId} → Template={template}");
    }

    static void SendPush(string userId, string message)
    {
        Console.WriteLine($"📲 [Push] User={userId} → Message='{message}'");
    }
}
