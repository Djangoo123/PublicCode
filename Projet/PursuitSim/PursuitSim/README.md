
# PursuitSim (.NET 8 Console)

Console simulation: Team of 3 runners over 1 km against a drone that patrols, detects, pursues, and hits (KO).
Built-in rules: escape routes, switch upon detection, single/double KO, drone destroyed then respawns after 5 minutes.

## Launch

```bash
dotnet run -- 1 # Scenario 1: Plain + Hedges
dotnet run -- 2 # Scenario 2: Urban Checkerboard
dotnet run -- 3 # Scenario 3: Overcast then final clearing
```

Output: every second → time, team status, number alive, Y of the "head", drone status/position, events.
