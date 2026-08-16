-- කලින් තිබුණ වැරදි role එකක් තියෙනවා නම් ඒක මකා දමමු
DELETE FROM AspNetUserRoles WHERE UserId = 'a75ebf25-df53-4bc2-8f71-888ffbf02c3b ';

-- අලුතින් Admin Role එක ලබා දෙමු
INSERT INTO AspNetUserRoles (UserId, RoleId) 
VALUES ('a75ebf25-df53-4bc2-8f71-888ffbf02c3b ', 'cfd-f10a113c1ba5 ');