-- Wishwa පරිශීලකයා සඳහා
UPDATE AspNetUsers 
SET UserName = 'wishwanimesh', PhoneNumber = '0711234567' 
WHERE Email = 'wishwanimesh@gmail.com';

-- Admin පරිශීලකයා සඳහා
UPDATE AspNetUsers 
SET UserName = 'admin1234', PhoneNumber = '0777654321' 
WHERE Email = 'admin@gmail.com';

-- Chanaka පරිශීලකයා සඳහා
UPDATE AspNetUsers 
SET UserName = 'chanakakavinda', PhoneNumber = '0755554444' 
WHERE Email = 'chanakavinda2000@gmail.com';

-- Nipuni පරිශීලකයා සඳහා
UPDATE AspNetUsers 
SET UserName = 'nipuninethmini', PhoneNumber = '0788889999' 
WHERE Email = 'nipuninethmini2002@gmail.com';
select * from AspNetUsers;
