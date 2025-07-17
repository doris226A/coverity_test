using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace SagaLib
{
    public static class JsonUtil
    {
        public static object Response { get; private set; }

        /// <summary>
        /// 将JSON转换为指定类型的对象
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="json">json字符串</param>
        /// <returns></returns>
        public static T ConvertToObject<T>(string json)
        {
            try
            {
                var jsetting = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, MissingMemberHandling = MissingMemberHandling.Ignore };
                jsetting.DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate;
                return JsonConvert.DeserializeObject<T>(json, jsetting);
            }
            catch
            {
                return default(T);
            }
        }


        /// <summary>
        /// 生成压缩的json 字符串
        /// </summary>
        /// <param name="obj">生成json的对象</param>
        /// <returns></returns>
        public static string ToJson(object obj)
        {
            JsonSerializerSettings setting = new JsonSerializerSettings();
            setting.NullValueHandling = NullValueHandling.Ignore;
            return JsonConvert.SerializeObject(obj, setting);
        }

        /// <summary>
        /// 生成压缩的json 字符串
        /// </summary>
        /// <param name="obj">生成json的对象</param>
        /// <returns></returns>
        public static string ToJson(object obj, string format)
        {
            IsoDateTimeConverter timeFormat = new IsoDateTimeConverter();
            timeFormat.DateTimeFormat = format;

            return JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented, timeFormat);

        }

        /// <summary>
        /// 格式化EASYUI DATAGRID JSON
        /// </summary>
        /// <param name="recordcount">总记录数</param>
        /// <param name="rows">每页记录的JSON格式</param>
        /// <returns></returns>
        public static string FormatJSONForEasyuiDataGrid(int recordcount, object rowsList)
        {
            return ToJson(new { total = recordcount, rows = rowsList }, "yyyy-MM-dd HH:mm:ss");
        }

        public static string FormatJSONForEasyuiDataGrid(int recordcount, object rowsList, string format, long datetime)
        {
            return ToJson(new { total = recordcount, rows = rowsList, Dt = datetime }, format);
        }
        public static string FormatJSONForEasyuiDataGrid(int recordcount, object rowsList, string format)
        {
            return ToJson(new { total = recordcount, rows = rowsList }, format);
        }


        public static DateTime JsonToDateTime(string jsonDate)
        {
            string value = jsonDate.Substring(6, jsonDate.Length - 8);
            DateTimeKind kind = DateTimeKind.Utc;
            int index = value.IndexOf('+', 1);
            if (index == -1)
                index = value.IndexOf('-', 1);
            if (index != -1)
            {
                kind = DateTimeKind.Local;
                value = value.Substring(0, index);
            }
            long javaScriptTicks = long.Parse(value, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture);
            long InitialJavaScriptDateTicks = (new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).Ticks;
            DateTime utcDateTime = new DateTime((javaScriptTicks * 10000) + InitialJavaScriptDateTicks, DateTimeKind.Utc);
            DateTime dateTime;
            switch (kind)
            {
                case DateTimeKind.Unspecified:
                    dateTime = DateTime.SpecifyKind(utcDateTime.ToLocalTime(), DateTimeKind.Unspecified);
                    break;
                case DateTimeKind.Local:
                    dateTime = utcDateTime.ToLocalTime();
                    break;
                default:
                    dateTime = utcDateTime;
                    break;
            }
            return dateTime;
        }

        public static T ConvertToObject<T>(T newClass)
        {
            throw new NotImplementedException();
        }
    }
}
