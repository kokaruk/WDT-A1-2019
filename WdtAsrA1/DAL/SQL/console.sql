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