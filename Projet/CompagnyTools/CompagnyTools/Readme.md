### Database synchronization

To synchronize the project when a database element is created or modified one must execute a command in **Visual Studio's NuGet console**. 
To do so open a NuGet console and place the prompt in the project root directory. 
Set CompagnyTools as startup project
Then type on screen:

```
Scaffold-DbContext -v -Connection "name=ConnectionString" -Provider Npgsql.EntityFrameworkCore.PostgreSQL -OutputDir Entities -ContextDir Context -Context EFCoreContext -Schemas public -Force -Project "CompagnyTools" 
```