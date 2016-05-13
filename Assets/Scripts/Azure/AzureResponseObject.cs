using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using RestSharp.Extensions;
using System.Reflection;

public class AzureResponseObject
{
   private CultureInfo _culture = CultureInfo.InvariantCulture;

   public bool Deserialize( string content )
   {
      var jsonValues = GetJsonValues( content );

      foreach ( var prop in GetType().GetProperties() )
      {
         if ( jsonValues.ContainsKey( prop.Name.ToLower() ) )
         {
            prop.SetValue( this, ConvertValue( prop.PropertyType, jsonValues[prop.Name.ToLower()] ), null );
         }
         else
         {
            return false;
         }
      }

      return true;
   }

   private Dictionary<string, string> GetJsonValues( string content )
   {
      var list = new Dictionary<string, string>();

      try
      {

         content = content.TrimStart( new[] { '[', ']', '{', '}' } );
         content = content.TrimEnd( new[] { '[', ']', '{', '}' } );

         var splitValues = content.Split( ',' );
         foreach ( var v in splitValues )
         {
            var colonIndex = v.IndexOf( ':' );
            var name = v.Substring( 0, colonIndex );
            var value = v.Substring( colonIndex, v.Count() - colonIndex );
            if ( !string.IsNullOrEmpty( name ) && !string.IsNullOrEmpty( value ) )
            {
               name = name.Trim( '\"' );
               value = value.Trim( '\"' );

               list[name.ToLower()] = value.ToLower();
            }
         }
      }
      catch { }

      return list;
   }

   private object ConvertValue( Type type, object value )
   {
      var stringValue = Convert.ToString( value, _culture );

      // check for nullable and extract underlying type
      //if ( type.IsGenericType && type.GetGenericTypeDefinition() == typeof( Nullable<> ) )
      //{
      //   // Since the type is nullable and no value is provided return null
      //   if ( String.IsNullOrEmpty( stringValue ) )
      //   {
      //      return null;
      //   }

      //   type = type.GetGenericArguments()[0];
      //}

      if ( type == typeof( System.Object ) && value != null )
      {
         type = value.GetType();
      }

      //if ( type.IsPrimitive )
      //{
      //   return value.ChangeType( type, _culture );
      //}
      //else if ( type.IsEnum )
      //{
      //   return type.FindEnumValue( stringValue, _culture );
      //}
      //else if ( type == typeof( Uri ) )
      if ( type == typeof( Uri ) )
      {
         return new Uri( stringValue, UriKind.RelativeOrAbsolute );
      }
      else if ( type == typeof( string ) )
      {
         return stringValue;
      }
      else if ( type == typeof( DateTime ) || type == typeof( DateTimeOffset ) )
      {
         DateTime dt;
         //if ( DateFormat.HasValue() )
         //{
         //   dt = DateTime.ParseExact( stringValue, DateFormat, _culture );
         //}
         //else
         //{
         // try parsing instead
         dt = stringValue.ParseJsonDate( _culture );
         //}

         if ( type == typeof( DateTime ) )
         {
            return dt;
         }
         else if ( type == typeof( DateTimeOffset ) )
         {
            return (DateTimeOffset)dt;
         }
      }
      else if ( type == typeof( Decimal ) )
      {
         if ( value is double )
            return (decimal)((double)value);

         return Decimal.Parse( stringValue, _culture );
      }
      else if ( type == typeof( Guid ) )
      {
         return string.IsNullOrEmpty( stringValue ) ? Guid.Empty : new Guid( stringValue );
      }
      else if ( type == typeof( TimeSpan ) )
      {
         return TimeSpan.Parse( stringValue );
      }
      //else if ( type.IsGenericType )
      //{
      //   var genericTypeDef = type.GetGenericTypeDefinition();
      //   if ( genericTypeDef == typeof( List<> ) )
      //   {
      //      return BuildList( type, value );
      //   }
      //   else if ( genericTypeDef == typeof( Dictionary<,> ) )
      //   {
      //      var keyType = type.GetGenericArguments()[0];

      //      // only supports Dict<string, T>()
      //      if ( keyType == typeof( string ) )
      //      {
      //         return BuildDictionary( type, value );
      //      }
      //   }
      //   else
      //   {
      //      // nested property classes
      //      return CreateAndMap( type, value );
      //   }
      //}
      //else
      //{
      //   // nested property classes
      //   return CreateAndMap( type, value );
      //}

      return null;
   }
}
