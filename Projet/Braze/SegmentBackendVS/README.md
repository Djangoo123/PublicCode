# SegmentBackend (VS project) — with Braze push

- .NET 8 Web API
- EF Core InMemory with seed
- Swagger on launch
- /api/segments/preview — JSON filter preview
- /api/segments/push-to-braze?tag=VIP_Test — pushes attribute `cohort_VIP_Test=true` to Braze for matching users (batch 75)

## Configure Braze
Set either environment variables **BRAZE_REST_ENDPOINT** and **BRAZE_REST_API_KEY**, or fill `appsettings.json`:

```json
"Braze": { "Endpoint": "https://rest.iad-03.braze.com", "ApiKey": "<YOUR_KEY>" }
```

## Run
Open `SegmentBackend.sln` in Visual Studio → F5 (Swagger opens).

## Test body
See previous JSON examples (VIP Premium Europe).