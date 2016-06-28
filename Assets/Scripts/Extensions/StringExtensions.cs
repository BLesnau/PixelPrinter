using System;
using UnityEngine;
using System.Globalization;
using System.Text.RegularExpressions;

public static class StringExtensions
{
   private static DateTime ParseFormattedDate( string input, CultureInfo culture )
   {
      var formats = new[]
      {
            "u",
            "s",
            "yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'",
            "yyyy-MM-ddTHH:mm:ssZ",
            "yyyy-MM-dd HH:mm:ssZ",
            "yyyy-MM-ddTHH:mm:ss",
            "yyyy-MM-ddTHH:mm:sszzzzzz",
            "yyyy-mm-ddThh:mm:ss[.mmm]",
            "M/d/yyyy h:mm:ss tt" // default format for invariant culture
         };

      DateTime date;
      if ( DateTime.TryParseExact( input, formats, culture, DateTimeStyles.None, out date ) )
      {
         return date;
      }

      if ( DateTime.TryParse( input, culture, DateTimeStyles.None, out date ) )
      {
         return date;
      }

      if ( DateTime.TryParse( input, out date ) )
      {
         return date;
      }

      return default( DateTime );
   }

   private static DateTime ExtractDate( string input, string pattern, CultureInfo culture )
   {
      DateTime dt = DateTime.MinValue;
      var regex = new Regex( pattern );
      if ( regex.IsMatch( input ) )
      {
         var matches = regex.Matches( input );
         var match = matches[0];
         var ms = Convert.ToInt64( match.Groups[1].Value );
         var epoch = new DateTime( 1970, 1, 1, 0, 0, 0, DateTimeKind.Utc );
         dt = epoch.AddMilliseconds( ms );

         // adjust if time zone modifier present
         if ( match.Groups.Count > 2 && !String.IsNullOrEmpty( match.Groups[3].Value ) )
         {
            var mod = DateTime.ParseExact( match.Groups[3].Value, "HHmm", culture );
            if ( match.Groups[2].Value == "+" )
            {
               dt = dt.Add( mod.TimeOfDay );
            }
            else
            {
               dt = dt.Subtract( mod.TimeOfDay );
            }
         }

      }
      return dt;
   }

   public static DateTime ParseJsonDate( this string input, CultureInfo culture )
   {
      input = input.Replace( "\n", "" );
      input = input.Replace( "\r", "" );

      //input = input.RemoveSurroundingQuotes();

      long unix;
      if ( Int64.TryParse( input, out unix ) )
      {
         var epoch = new DateTime( 1970, 1, 1, 0, 0, 0, DateTimeKind.Utc );
         return epoch.AddSeconds( unix );
      }

      if ( input.Contains( "/Date(" ) )
      {
         return ExtractDate( input, @"\\?/Date\((-?\d+)(-|\+)?([0-9]{4})?\)\\?/", culture );
      }

      if ( input.Contains( "new Date(" ) )
      {
         input = input.Replace( " ", "" );
         // because all whitespace is removed, match against newDate( instead of new Date(
         return ExtractDate( input, @"newDate\((-?\d+)*\)", culture );
      }

      return ParseFormattedDate( input, culture );
   }
}