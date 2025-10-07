# Kafka POC + SegmentBackend

## Structure
- ProducerApp : envoie des événements test
- ConsumerApp : lit et affiche brut
- NotificationService : consomme et simule un envoi de campagne
- SegmentBackend : API .NET qui prend un segment JSON, filtre des users et publie les `UserMatchedSegment` dans Kafka

## Lancer Kafka local
```bash
docker compose up -d
docker exec -it $(docker ps -qf "ancestor=confluentinc/cp-kafka:7.5.0") \
  kafka-topics --create --bootstrap-server localhost:9092 --replication-factor 1 --partitions 1 --topic user-segments
```

## Tester SegmentBackend
1. Démarre l'API :
```bash
dotnet run --project SegmentBackend
```
2. Appelle l'endpoint :
```bash
curl -X POST http://localhost:5000/segments/run -H "Content-Type: application/json" -d '{"Plan":"Premium","Country":"FR","SegmentId":"premium_fr"}'
```

## Résultat attendu
- L'API retourne le nombre de users matchés.
- Pour chaque user, un message Kafka est produit.
- NotificationService lit les events et simule l'envoi de campagne.
