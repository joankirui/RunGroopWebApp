# RunGroopWebApp
Rungroops is an online platform for runners. This platform will help you to find clubs, find races, and meet other runners in your area.
# Set Up
1. Go into directory where you plan on keeping project and run.
  git fork https://github.com/joankirui/RunGroopWebApp.git.
  
2. Create a local database.
3.Add connection string to app settings.json. It will look something like this:

  Data Source=DESKTOP-EI2TOGP\\SQLEXPRESS;Initial Catalog=RunGroops;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False
  
4. Register for a Cloudinary Account (%100 free) and add Cloudname, ApiKey, and Api secret to appsettings.json.
