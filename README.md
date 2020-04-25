# How to get started
- Install PostgreSQL
- Create a DB (for example: market_locator)
- Create a user (for example: market_locator_app)
- Grant all rights to this user for this particular DB
- Seed the DB with create_database.sql
- Create the appsettings.Developement.json file (not versioned because of the password it contains). There is an example file you can copy.
- Create the google_api_secret.json file (see https://www.youtube.com/watch?v=afTiNU6EoA8 for an example how to get such a file, also don't forget to give read access to the sheet to the service user listed in the file)