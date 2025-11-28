
# SegmentSuite (Backend API & logique)

Inclus :
- **Segments CRUD**: save, update, get, list
- **Preview**: count + sample avec timeout
- **Opérateurs**: eq, neq, lt/lte/gt/gte, contains/startsWith/endsWith, in/nin, between, exists/notExists
- **Projection Braze via jobs**: Add/Remove flag, Add/Remove cohort_id array
- **Background worker**: exécute la queue (75 par batch)

## Lancer
- Ouvrir `SegmentSuite.sln` dans Visual Studio → F5
- Swagger: http://localhost:5288/swagger

## Endpoints
- `POST /api/segments/preview?take=25&timeoutMs=5000`
- `POST /api/segments/save`
- `PUT /api/segments/{id}`
- `GET /api/segments/{id}`
- `GET /api/segments/list`
- `POST /api/segments/{id}/queue-braze?mode=flag&key=cohort_VIP&action=add`
  - ou `mode=array&key=c-2025-08-AB12&action=add`

## Braze (optionnel)
Configurer `appsettings.json` ou variables d'environnement :
- `BRAZE_REST_ENDPOINT=https://rest.iad-03.braze.com`
- `BRAZE_REST_API_KEY=xxxx`

## Tests
Projet `SegmentBackend.Tests` (xUnit) avec un test de composition.
