using System;
using System.ComponentModel;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace Api {

  public class SessionHelper {
      private readonly Func<ISession>  sessionGetter;

      public SessionHelper(Func<ISession> sessionGetter) {
        this.sessionGetter = sessionGetter;
      }

      // public void Set(string key, object value) {
      //   TypeConverter obj = TypeDescriptor.GetConverter(value.GetType());
      //   byte[] bt = (byte[])obj.ConvertTo(value, typeof(byte[]));
      //   session.Set(key, bt);
      // }


      // public object Get<T>(string key) {

      //   var bytes = session.Get(key);


      //   TypeConverter obj = TypeDescriptor.GetConverter(value.GetType());
      //   byte[] bt = (byte[])obj.ConvertTo(value, typeof(byte[]));
      //   session.Set(key, bt);

      //   return session.Get(key).GetT;
      // }

      public void SetString(string key, string value) {
        sessionGetter().Set(key, Encoding.UTF8.GetBytes(value));
      }
      public string GetString(string key) {
        sessionGetter().TryGetValue(key, out var result);
        if (result == null) return "";
        return Encoding.UTF8.GetString(result);
      }
  }
}
