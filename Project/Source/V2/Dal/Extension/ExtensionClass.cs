using System;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Api213.V2.Dal.Extension
{
    /// <summary>
    /// 
    /// </summary>
    public static class ExtensionClass
    {
        /// <summary>
        /// order by list by string sort
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        public static IQueryable<T> Sort<T>(this IQueryable<T> source, string sort)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (string.IsNullOrEmpty(sort))
                throw new ArgumentNullException(nameof(sort));

            source = source.OrderBy(To_Core(sort));

            return source;
        }

        /// <summary>
        /// GET /x?fields=id,subject,customer_name,updated_at 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="fields"></param>
        /// <param name="objectType"></param>
        /// <returns></returns>
        public static IQueryable LimitingFields<T>(this IQueryable<T> source, string fields, Type objectType)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (string.IsNullOrEmpty(fields))
                throw new ArgumentNullException(nameof(fields));

            IQueryable query = source
                .Select(string.Empty + To_Corefields(fields, objectType));
            return query;
        }

        /// <summary>
        /// +id,-name  TO id asc,name desc
        /// soporte a System.Linq.Dynamic.Core
        /// </summary>
        /// <param name="sortExpressions">sort</param>
        /// <returns>id asc,name desc</returns>
        private static string To_Core(string sortExpressions)
        {
            var sortables = sortExpressions.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            var sortcore = string.Empty;
            var concat = string.Empty;
            foreach (var item in sortables)
            {
                if (item.StartsWith("-"))
                {
                    sortcore += concat + item.Replace("-", string.Empty) + " desc";
                }

                if (!item.StartsWith("-"))
                {
                    sortcore += concat + item.Replace("+", string.Empty) + " asc";
                }

                concat = ",";
            }

            return sortcore;
        }

        /// <summary>
        /// id,subject,customer_name,updated_at
        /// to
        /// "new(Id,Subject,Customer_name,Updated_at)"
        /// </summary>
        /// <param name="limitingFields"></param>
        /// <param name="objectType"></param>
        /// <returns></returns> 
        private static string To_Corefields(string limitingFields, Type objectType)
        {
            var sortables = limitingFields.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            var fieldscore = string.Empty;
            var concat = string.Empty;
            foreach (var item in sortables)
            {
                fieldscore += concat + GetPropertyFrom(item, objectType);

                concat = ",";
            }

            return "new(" + fieldscore + ")";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private static string GetPropertyFrom(string item, Type type)
        {
            var members = type.GetMembers();
            var existfield = members.Where(x => string.Equals(
                x.Name,
                    item,
                    StringComparison.CurrentCultureIgnoreCase))
                .Select(x => x.Name)
                .FirstOrDefault();
            return existfield;
        }

        /*
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static object ToNonAnonymousList<T>(this List<T> list, Type t)
        {
            //define system Type representing List of objects of T type:
            Type genericType = typeof(List<>).MakeGenericType(t);

            //create an object instance of defined type:
            object l = Activator.CreateInstance(genericType);

            //get method Add from from the list:
            MethodInfo addMethod = l.GetType().GetMethod("Add");

            //loop through the calling list:
            foreach (T item in list)
            {
                //convert each object of the list into T object by calling extension ToType<T>()
                //Add this object to newly created list:
                addMethod.Invoke(l, new[] {item.ToType(t)});
            }

            //return List of T objects:
            return l;
        }
        */
        /*
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object ToType<T>(this object obj, T type)
        {
            //create instance of T type object:
            object tmp = Activator.CreateInstance(Type.GetType(type.ToString()));

            //loop through the properties of the object you want to covert:          
            foreach (PropertyInfo pi in obj.GetType().GetProperties())
            {
                //fix dynamic
                if (!pi.PropertyType.ToString().Contains("Object"))
                {
                    //get the value of property and try to assign it to the property of T type object:
                    tmp.GetType().GetProperty(pi.Name).SetValue(tmp, pi.GetValue(obj, null), null);
                }
            }

            //return the T type object:         
            return tmp;
        }*/
    }
}