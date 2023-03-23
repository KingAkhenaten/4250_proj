## BucHunt data is broken into a few tables.

# Users
Contains data about individual users
- table 'user'
- contains a unique row ID (primary key)
- 'email' contains their user e-mail, limit 64 characters
- 'phone' contains their phone number in string form, limit 24 characters. Client validates.
- each e-mail and phone number must form a unique key. Client must enforce assuming the database cannot.
  - Database may be able to have a constraint to enforce this. Implementation not yet known.
- 'username' contains their username, limit 32 characters

# Admins
Contains data about admin access
- table 'admin'
- To be determined

# Hunt
Contains data identifying the root of a hunt
- table 'hunt'
- 'huntid' contains unique id for a hunt, integer (primary key)
- 'huntname' contains a display name for this hunt, limit 64 characters
- 'hunturl' contains landing page for this hunt, limit 256 characters

# Location
Contains data identifying locations and the hunts for which they associate
- table 'location'
- contains a unique row ID (primary key)
- 'huntid' contains the hunt id to which this location is associated
- 'location' contains the display name for this location i.e. 'Culp Centre', limit 100 characters
- 'lat' contains the GPS location latitude, real
- 'lon' contains the GPS location longitude, real
- 'task' contains the task for which this location is associated, limit 150 characters
- 'accesscode' contains the access code found at this location, format unknown (likely string)
- 'qrcode' contains the QR code payload found at this location, format unknown (likely string)
- 'answer' contains the answer for this location, limit 100 characters

# Hunters
Contains data linking users to the hunts they participate in
- table 'hunters'
- contains a unique row ID (primary key)
- 'huntid' contains a hunt id for this user
- 'userid' contains a user row ID for the user referenced to be in this hunt
- all rows in this table are unique, referencing a hunt, and one user in that hunt
  - A user may be in multiple hunts
  - Hunts can have multiple users

insert into location values (null,1,'Millenium Centre',0,0,'How many TVs are on the first floor?',null,null,null);
insert into location values (null,1,'Library',0,0,'How many printers are available for student use on the first floor?',null,null,null);
insert into location values (null,1,'Administration Building',0,0,'What room number is the dean''s room?',null,null,null);
insert into location values (null,1,'Roy S. Nicks Hall',0,0,'What is the name of the first hallway on the fourth floor?',null,null,null);
insert into location values (null,1,'Basler Centre for Physical Activity',0,0,'How tall is the rock-climbing wall?',null,null,null);
insert into location values (null,1,'Burleson Hall',0,0,'How many floors are there?',null,null,null);
insert into location values (null,1,'ETSU Bell Tower',0,0,'How many bells are there?',null,null,null);
insert into location values (null,1,'D. P. Culp Centre',0,0,'What store is directly in front of the library-side entrance?',null,null,null);
insert into location values (null,1,'Sam Wilson Hall',0,0,'What is the number of the first classroom on the right when entering through the rightmost door from the quad?',null,null,null);
insert into location values (null,1,'Brown Hall',0,0,'What kind of tree is planted in the centre courtyard? Hint: check the lobby plaque.',null,null,null);