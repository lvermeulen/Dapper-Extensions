using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace DapperExtensions.Test.Helpers
{
    public class Protected
    {
        private readonly object _obj;

        public Protected(object obj)
        {
	        _obj = obj ?? throw new ArgumentException("object cannot be null.", nameof(obj));
        }

        public static Expression Null<T>()
        {
            Expression<Func<Type>> expr = () => typeof(T);
            return expr.Body;
        }

        public void RunMethod(string name, params object[] parameters)
        {
            InvokeMethod(name, null, parameters);
        }

        public T RunMethod<T>(string name, params object[] parameters)
        {
            return (T)InvokeMethod(name, null, parameters);
        }

        public void RunGenericMethod(string name, Type[] genericTypes, params object[] parameters)
        {
            InvokeMethod(name, genericTypes, parameters);
        }

        public TResult RunGenericMethod<TResult>(string name, Type[] genericTypes, params object[] parameters)
        {
            return (TResult)InvokeMethod(name, genericTypes, parameters);
        }

        public object InvokeMethod(string name, Type[] genericTypes, object[] parameters)
        {
            var pa = parameters.Select(p =>
            {
                if (p is ConstantExpression)
                {
                    return null;
                }

                return p;
            }).ToArray();
            var method = GetMethod(name, parameters);
            try
            {
                if (genericTypes != null && genericTypes.Any())
                {
                    method = method.MakeGenericMethod(genericTypes);
                }

                return method.Invoke(_obj, pa);
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException ?? ex;
            }
        }

        public MethodInfo GetMethod(string name, object[] parameters)
        {
            var types = parameters.Select(p =>
            {
                if (p is ConstantExpression constantExpression)
                {
                    return (Type)constantExpression.Value;
                }

                return p.GetType();
            }).ToArray();
            var method = _obj.GetType().GetMethod(name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public, null, types, null);
            if (method == null)
            {
                throw new ArgumentException($"{name} was not found in {_obj.GetType()}.", name);
            }

            return method;
        }
    }
}