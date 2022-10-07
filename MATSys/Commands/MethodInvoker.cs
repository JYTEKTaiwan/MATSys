using System.Reflection;

namespace MATSys.Commands
{
    /// <summary>
    /// Method Invoker class
    /// </summary>
    public abstract class MethodInvoker
    {
        public abstract object? Invoke(params object[]? parameter);

        /// <summary>
        /// Create new Invoker instance 
        /// </summary>
        /// <param name="target">taget object</param>
        /// <param name="mi">method information</param>
        /// <returns>Invoker instance</returns>
        public static MethodInvoker Create(object target, MethodInfo mi)
        {
            try
            {
                var t = GetInvokerType(mi);
                var obj = Activator.CreateInstance(t, target, mi.Name);
                if (obj == null)
                {
                    throw new NullReferenceException();
                }
                else
                {
                    return (MethodInvoker)obj;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }
        /// <summary>
        /// Derive the correct Invoker instance
        /// </summary>
        /// <param name="mi">MethodInfo</param>
        /// <returns>Invoker Type</returns>
        /// <exception cref="ArgumentException"></exception>        
        private static Type GetInvokerType(MethodInfo mi)
        {
            var isNullReturn = mi.ReturnType.FullName == "System.Void";
            var parameters = mi.GetParameters().Select(x => x.ParameterType).ToArray();
            var returnType = mi.ReturnType;
            switch (parameters.Length)
            {
                case 0:
                    if (isNullReturn)
                    {
                        return typeof(Invoker);
                    }
                    else
                    {
                        return typeof(InvokerWithReturn<>).MakeGenericType(returnType);
                    }
                case 1:
                    if (isNullReturn)
                    {
                        return typeof(Invoker<>).MakeGenericType(parameters);
                    }
                    else
                    {
                        return typeof(InvokerWithReturn<,>).MakeGenericType(parameters.Append(returnType).ToArray());
                    }
                case 2:
                    if (isNullReturn)
                    {
                        return typeof(Invoker<,>).MakeGenericType(parameters);
                    }
                    else
                    {
                        return typeof(InvokerWithReturn<,,>).MakeGenericType(parameters.Append(returnType).ToArray());
                    }
                case 3:
                    if (isNullReturn)
                    {
                        return typeof(Invoker<,,>).MakeGenericType(parameters);
                    }
                    else
                    {
                        return typeof(InvokerWithReturn<,,,>).MakeGenericType(parameters.Append(returnType).ToArray());
                    }
                case 4:
                    if (isNullReturn)
                    {
                        return typeof(Invoker<,,,>).MakeGenericType(parameters);
                    }
                    else
                    {
                        return typeof(InvokerWithReturn<,,,,>).MakeGenericType(parameters.Append(returnType).ToArray());
                    }
                case 5:
                    if (isNullReturn)
                    {
                        return typeof(Invoker<,,,,>).MakeGenericType(parameters);
                    }
                    else
                    {
                        return typeof(InvokerWithReturn<,,,,,>).MakeGenericType(parameters.Append(returnType).ToArray());
                    }
                case 6:
                    if (isNullReturn)
                    {
                        return typeof(Invoker<,,,,,>).MakeGenericType(parameters);
                    }
                    else
                    {
                        return typeof(InvokerWithReturn<,,,,,,>).MakeGenericType(parameters.Append(returnType).ToArray());
                    }
                case 7:
                    if (isNullReturn)
                    {
                        return typeof(Invoker<,,,,,,>).MakeGenericType(parameters);
                    }
                    else
                    {
                        return typeof(InvokerWithReturn<,,,,,,,>).MakeGenericType(parameters.Append(returnType).ToArray());
                    }

                default:
                    throw new ArgumentException("Parameter length is incompatible");
            }
        }
    }
    internal class Invoker : MethodInvoker
    {
        private readonly Action? _invoker;
        public Invoker(object target, string name)
        {
            _invoker = Delegate.CreateDelegate(typeof(Action), target, name) as Action;
        }
        public override object? Invoke(params object[]? parameter)
        {
            if (parameter == null)
            {
                throw new ArgumentOutOfRangeException("parameter is null");
            }
            _invoker?.Invoke();
            return null;
        }
    }
    internal class Invoker<T1> : MethodInvoker
    {
        private readonly Action<T1>? _invoker;
        public Invoker(object target, string name)
        {
            _invoker = Delegate.CreateDelegate(typeof(Action<T1>), target, name) as Action<T1>;
        }
        public override object? Invoke(params object[]? parameter)
        {
            if (parameter == null || parameter.Length < 1)
            {
                throw new ArgumentOutOfRangeException("parameter too few");
            }
            _invoker?.Invoke((T1)parameter[0]);
            return null;

        }
    }
    internal class Invoker<T1, T2> : MethodInvoker
    {
        private readonly Action<T1, T2>? _invoker;
        public Invoker(object target, string name)
        {
            _invoker = Delegate.CreateDelegate(typeof(Action<T1, T2>), target, name) as Action<T1, T2>;
        }
        public override object? Invoke(params object[]? parameter)
        {
            if (parameter == null || parameter.Length < 2)
            {
                throw new ArgumentOutOfRangeException("parameter too few");
            }
            _invoker?.Invoke((T1)parameter[0], (T2)parameter[1]);
            return null;
        }
    }
    internal class Invoker<T1, T2, T3> : MethodInvoker
    {
        private readonly Action<T1, T2, T3>? _invoker;
        public Invoker(object target, string name)
        {
            _invoker = Delegate.CreateDelegate(typeof(Action<T1, T2, T3>), target, name) as Action<T1, T2, T3>;
        }
        public override object? Invoke(params object[]? parameter)
        {
            if (parameter == null || parameter.Length < 3)
            {
                throw new ArgumentOutOfRangeException("parameter too few");
            }
            _invoker?.Invoke((T1)parameter[0], (T2)parameter[1], (T3)parameter[2]);
            return null;
        }
    }
    internal class Invoker<T1, T2, T3, T4> : MethodInvoker
    {
        private readonly Action<T1, T2, T3, T4>? _invoker;
        public Invoker(object target, string name)
        {
            _invoker = Delegate.CreateDelegate(typeof(Action<T1, T2, T3, T4>), target, name) as Action<T1, T2, T3, T4>;
        }
        public override object? Invoke(params object[]? parameter)
        {
            if (parameter == null || parameter.Length < 4)
            {
                throw new ArgumentOutOfRangeException("parameter too few");
            }
            _invoker?.Invoke((T1)parameter[0], (T2)parameter[1], (T3)parameter[2], (T4)parameter[3]);
            return null;
        }
    }
    internal class Invoker<T1, T2, T3, T4, T5> : MethodInvoker
    {
        private readonly Action<T1, T2, T3, T4, T5>? _invoker;
        public Invoker(object target, string name)
        {
            _invoker = Delegate.CreateDelegate(typeof(Action<T1, T2, T3, T4, T5>), target, name) as Action<T1, T2, T3, T4, T5>;
        }
        public override object? Invoke(params object[]? parameter)
        {
            if (parameter == null || parameter.Length < 5)
            {
                throw new ArgumentOutOfRangeException("parameter too few");
            }
            _invoker?.Invoke(
                (T1)parameter[0],
                (T2)parameter[1],
                (T3)parameter[2],
                (T4)parameter[3],
                (T5)parameter[4]);
            return null;
        }
    }
    internal class Invoker<T1, T2, T3, T4, T5, T6> : MethodInvoker
    {
        private readonly Action<T1, T2, T3, T4, T5, T6>? _invoker;
        public Invoker(object target, string name)
        {
            _invoker = Delegate.CreateDelegate(typeof(Action<T1, T2, T3, T4, T5, T6>), target, name) as Action<T1, T2, T3, T4, T5, T6>;
        }
        public override object? Invoke(params object[]? parameter)
        {
            if (parameter == null || parameter.Length < 6)
            {
                throw new ArgumentOutOfRangeException("parameter too few");
            }
            _invoker?.Invoke(
                (T1)parameter[0],
                (T2)parameter[1],
                (T3)parameter[2],
                (T4)parameter[3],
                (T5)parameter[4],
                (T6)parameter[5]);
            return null;
        }
    }
    internal class Invoker<T1, T2, T3, T4, T5, T6, T7> : MethodInvoker
    {
        private readonly Action<T1, T2, T3, T4, T5, T6, T7>? _invoker;
        public Invoker(object target, string name)
        {
            _invoker = Delegate.CreateDelegate(typeof(Action<T1, T2, T3, T4, T5, T6, T7>), target, name) as Action<T1, T2, T3, T4, T5, T6, T7>;
        }
        public override object? Invoke(params object[]? parameter)
        {
            if (parameter == null || parameter.Length < 7)
            {
                throw new ArgumentOutOfRangeException("parameter too few");
            }
            _invoker?.Invoke(
                (T1)parameter[0],
                (T2)parameter[1],
                (T3)parameter[2],
                (T4)parameter[3],
                (T5)parameter[4],
                (T6)parameter[5],
                (T7)parameter[6]);
            return null;
        }
    }
    internal class InvokerWithReturn<Tout> : MethodInvoker
    {
        private readonly Func<Tout>? _invoker;

        public InvokerWithReturn(object target, string name)
        {
            _invoker = Delegate.CreateDelegate(typeof(Func<Tout>), target, name) as Func<Tout>;
        }

        public override object? Invoke(params object[]? parameter)
        {
            if (parameter == null)
            {
                throw new ArgumentOutOfRangeException("parameter is null");
            }
            if (_invoker == null)
            {
                throw new NullReferenceException("invoker is null");
            }
            return _invoker.Invoke();
        }
    }
    internal class InvokerWithReturn<T1, Tout> : MethodInvoker
    {
        private readonly Func<T1, Tout>? _invoker;

        public InvokerWithReturn(object target, string name)
        {
            _invoker = Delegate.CreateDelegate(typeof(Func<T1, Tout>), target, name) as Func<T1, Tout>;
        }

        public override object? Invoke(params object[]? parameter)
        {
            if (parameter == null || parameter.Length < 1)
            {
                throw new ArgumentOutOfRangeException("parameter too few");
            }
            if (_invoker == null)
            {
                throw new NullReferenceException("invoker is null");
            }
            return _invoker.Invoke((T1)parameter[0]);
        }
    }
    internal class InvokerWithReturn<T1, T2, Tout> : MethodInvoker
    {
        private readonly Func<T1, T2, Tout>? _invoker;

        public InvokerWithReturn(object target, string name)
        {
            _invoker = Delegate.CreateDelegate(typeof(Func<T1, T2, Tout>), target, name) as Func<T1, T2, Tout>;
        }

        public override object? Invoke(params object[]? parameter)
        {
            if (parameter == null || parameter.Length < 2)
            {
                throw new ArgumentOutOfRangeException("parameter too few");
            }
            if (_invoker == null)
            {
                throw new NullReferenceException("invoker is null");
            }
            return _invoker.Invoke(
                (T1)parameter[0],
                (T2)parameter[1]);
        }
    }
    internal class InvokerWithReturn<T1, T2, T3, Tout> : MethodInvoker
    {
        private readonly Func<T1, T2, T3, Tout>? _invoker;

        public InvokerWithReturn(object target, string name)
        {
            _invoker = Delegate.CreateDelegate(typeof(Func<T1, T2, T3, Tout>), target, name) as Func<T1, T2, T3, Tout>;
        }

        public override object? Invoke(params object[]? parameter)
        {
            if (parameter == null || parameter.Length < 3)
            {
                throw new ArgumentOutOfRangeException("parameter too few");
            }
            if (_invoker == null)
            {
                throw new NullReferenceException("invoker is null");
            }
            return _invoker!.Invoke(
                (T1)parameter[0],
                (T2)parameter[1],
                (T3)parameter[2]);
        }
    }
    internal class InvokerWithReturn<T1, T2, T3, T4, Tout> : MethodInvoker
    {
        private readonly Func<T1, T2, T3, T4, Tout>? _invoker;

        public InvokerWithReturn(object target, string name)
        {
            _invoker = Delegate.CreateDelegate(typeof(Func<T1, T2, T3, T4, Tout>), target, name) as Func<T1, T2, T3, T4, Tout>;
        }

        public override object? Invoke(params object[]? parameter)
        {
            if (parameter == null || parameter.Length < 4)
            {
                throw new ArgumentOutOfRangeException("parameter too few");
            }
            if (_invoker == null)
            {
                throw new NullReferenceException("invoker is null");
            }
            return _invoker!.Invoke(
                (T1)parameter[0],
                (T2)parameter[1],
                (T3)parameter[2],
                (T4)parameter[3]);
        }
    }
    internal class InvokerWithReturn<T1, T2, T3, T4, T5, Tout> : MethodInvoker
    {
        private readonly Func<T1, T2, T3, T4, T5, Tout>? _invoker;

        public InvokerWithReturn(object target, string name)
        {
            _invoker = Delegate.CreateDelegate(typeof(Func<T1, T2, T3, T4, T5, Tout>), target, name) as Func<T1, T2, T3, T4, T5, Tout>;
        }

        public override object? Invoke(params object[]? parameter)
        {
            if (parameter == null || parameter.Length < 5)
            {
                throw new ArgumentOutOfRangeException("parameter too few");
            }
            if (_invoker == null)
            {
                throw new NullReferenceException("invoker is null");
            }
            return _invoker!.Invoke(
                (T1)parameter[0],
                (T2)parameter[1],
                (T3)parameter[2],
                (T4)parameter[3],
                (T5)parameter[4]);
        }
    }
    internal class InvokerWithReturn<T1, T2, T3, T4, T5, T6, Tout> : MethodInvoker
    {
        private readonly Func<T1, T2, T3, T4, T5, T6, Tout>? _invoker;

        public InvokerWithReturn(object target, string name)
        {
            _invoker = Delegate.CreateDelegate(typeof(Func<T1, T2, T3, T4, T5, T6, Tout>), target, name) as Func<T1, T2, T3, T4, T5, T6, Tout>;
        }

        public override object? Invoke(params object[]? parameter)
        {
            if (parameter == null || parameter.Length < 6)
            {
                throw new ArgumentOutOfRangeException("parameter too few");
            }
            if (_invoker == null)
            {
                throw new NullReferenceException("invoker is null");
            }
            return _invoker!.Invoke(
                (T1)parameter[0],
                (T2)parameter[1],
                (T3)parameter[2],
                (T4)parameter[3],
                (T5)parameter[4],
                (T6)parameter[5]);
        }
    }
    internal class InvokerWithReturn<T1, T2, T3, T4, T5, T6, T7, Tout> : MethodInvoker
    {
        private readonly Func<T1, T2, T3, T4, T5, T6, T7, Tout>? _invoker;

        public InvokerWithReturn(object target, string name)
        {
            _invoker = Delegate.CreateDelegate(typeof(Func<T1, T2, T3, T4, T5, T6, T7, Tout>), target, name) as Func<T1, T2, T3, T4, T5, T6, T7, Tout>;
        }

        public override object? Invoke(params object[]? parameter)
        {
            if (parameter == null || parameter.Length < 7)
            {
                throw new ArgumentOutOfRangeException("parameter too few");
            }
            if (_invoker == null)
            {
                throw new NullReferenceException("invoker is null");
            }
            return _invoker!.Invoke(
                (T1)parameter[0],
                (T2)parameter[1],
                (T3)parameter[2],
                (T4)parameter[3],
                (T5)parameter[4],
                (T6)parameter[5],
                (T7)parameter[6]);
        }
    }

}
