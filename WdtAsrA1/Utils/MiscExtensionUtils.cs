using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using WdtAsrA1.Model;

namespace WdtAsrA1.Utils
{
    public static class MiscExtensionUtils
    {
        public static bool IsWithinMaxValue(this int value, int max, int min = 1) => value >= min && value <= max;

        // pad menu greetings string with '=' char
        public static string MenuHeaderPad(this string value) => string.Empty.PadLeft(value.Length, '=');

        public static SqlConnection CreateConnection(this string connectionString) =>
            new SqlConnection(connectionString);

        public static SqlCommand CreateProcedureCommand(this SqlConnection con,
            string procedure, Dictionary<string, dynamic> connParams = null)
        {
            var command = con.CreateCommand();
            command.CommandText = procedure;
            command.CommandType = CommandType.StoredProcedure;
            if (connParams != null) command.FillParams(connParams);
            return command;
        }

        internal static void FillParams(this SqlCommand command, Dictionary<string, dynamic> connParams)
        {
            foreach (var (key, value) in connParams)
            {
                command.Parameters.AddWithValue(key, value);
            }
        }

        internal static void SlotsListOutput(this StringBuilder slotsListOutput, List<Slot> slots)
        {
            slotsListOutput.Append($"{Environment.NewLine} --- List slots ---");

            slotsListOutput.Append(
                $"{Environment.NewLine}{"Room name",-11}{"Start time",-16}{"End time",-16}{"Staff ID",-14}Bookings");

            slots
                .ForEach(s =>
                    slotsListOutput.Append(
                        $"{Environment.NewLine}{s.RoomID,-11}{$"{s.StartTime:HH:mm}",-16}{$"{s.StartTime.AddMinutes(Program.SlotDuration):HH:mm}",-16}{s.StaffID,-14}{s.BookedInStudentId}")
                );
        }
    }
}