using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace ShowManagement.Core.Extensions
{
    public static class ObjectExtensions
    {
        public static string ExtractPropertyName<I,T>(this I instance, Expression<Func<I,T>> propertyExpression)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }

            if (propertyExpression == null)
            {
                throw new ArgumentNullException("propertyExpression");
            }

            var memberExpression = propertyExpression.Body as MemberExpression;
            if (memberExpression == null)
            {
                throw new ArgumentException("The member access expression does not access a property.", "propertyExpression");
            }

            var property = memberExpression.Member as PropertyInfo;
            if (property == null)
            {
                throw new ArgumentException("The expression is not a member access expression.", "propertyExpression");
            }

            return memberExpression.Member.Name;
        }

        public static T Clone<T>(this T source)
        {
            if (!typeof(T).IsSerializable)
            {
                throw new ArgumentException("They type must be serializable.", "source");
            }

            T clone = default(T);

            if (!Object.ReferenceEquals(source, null))
            {
                IFormatter formatter = new BinaryFormatter();
                Stream stream = new MemoryStream();
                using (stream)
                {
                    formatter.Serialize(stream, source);
                    stream.Seek(0, SeekOrigin.Begin);
                    clone = (T)formatter.Deserialize(stream);
                }
            }

            return clone;
        }
    }
}
