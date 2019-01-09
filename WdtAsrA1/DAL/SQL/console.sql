-- all rooms selection
create proc dbo.[all rooms]
as
begin
  select *
  from Room
end

  -- time procedure
  create proc dbo.[list slots for date] @Start as DATETIME2, @End as DATETIME2
  as
  SELECT *
  FROM Slot
  where StartTime >= @Start
    and StartTime < @End;
GO

-- slots for stuff member
create proc dbo.[list slots for staff] @StaffID as nvarchar(8), @CurrentTime as DATETIME2
as
SELECT *
FROM Slot
where StaffID = @StaffID
  and Slot.StartTime > @CurrentTime
order by Slot.StartTime ASC
GO

-- list staff procedure
create proc dbo.[list users]
as
select *
from dbo.[User];
go

-- get user by id
create proc dbo.[get user] @UserID as nvarchar(8)
as
select *
from dbo.[User]
where UserID = @UserID;
go

-- see room availability
create proc dbo.[check room availability] @Start as DATETIME2, @End as DATETIME2, @RoomID as nvarchar(10)
as
select *
from Slot
where StartTime >= @Start
  and StartTime < @End
  and RoomID = @RoomID;
go

-- add new slot
create proc dbo.[add new slot] @RoomID as nvarchar(10), @StartTime as DATETIME2, @StaffID as NVARCHAR(8)
as
insert into Slot (RoomID, StartTime, StaffID)
values (@RoomID, @StartTime, @StaffID);
go



-- remove slot
create proc dbo.[delete slot] @RoomID as nvarchar(10), @StartTime as DATETIME2
as
delete
from Slot
where RoomID = @RoomID
  and StartTime = @StartTime;
go



-- book slot
create proc dbo.[book slot] @RoomID as nvarchar(10), @StartTime as DATETIME2, @StudentID as nvarchar(8)
as
update Slot
set BookedInStudentID = @StudentID
where RoomID = @RoomID
  and StartTime = @StartTime;
  go

-- unbook slot
create proc dbo.[unbook slot] @RoomID as nvarchar(10), @StartTime as DATETIME2
as 
  update Slot
  set BookedInStudentID = null
  where RoomID = @RoomID
    and StartTime = @StartTime;
go

with b as (
  SELECT *
  FROM Slot
  where StartTime > '2019-01-10' and StartTime < '2019-01-11'  
)
SELECT RoomID, Count(RoomID)
       AS NumOccurrences
FROM b
GROUP BY RoomID
HAVING ( COUNT(RoomID) = 1 )
