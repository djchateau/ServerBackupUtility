
using System;
using System.Collections;
using System.Reflection;

namespace Schedule
{
	/// <summary>
	/// IParameterSetter represents a serialized parameter list.  This is used to provide a partial specialized
	/// method call.  This is useful for remote invocation of method calls.  For example if you have a method with
	/// 3 parameters.  The first 2 might represent static data such as a report and a storage location.  The third
	/// might be the time that the report is invoked, which is only known when the method is invoked.  Using this,
	/// you just pass the method and the first 2 parameters to a timer object, which supplies the 3rd parameter.
	/// Without these objects, you would have to generate a custom object type for each method you wished to 
	/// execute in this manner and store the static parameters as instance variables.  
	/// </summary>
	public interface IParameterSetter
	{
		/// <summary>
		/// This resets the setter to the beginning.  It is used for setters that rely on positional state
		/// information.  It is called prior to setting any method values.
		/// </summary>
		void Reset();

		/// <summary>
		/// This method is used to both query support for setting a parameter and actually set the value.
		/// True is returned if the parameter passed in is updated.
		/// </summary>
		/// <param name="parameterInfo">The reflection information about this parameter.</param>
		/// <param name="parameterLocation">The location of the parameter in the parameter list.</param>
		/// <param name="parameter">The parameter object</param>
		/// <returns>true if the parameter is matched and false otherwise</returns>
		bool GetParameterValue(ParameterInfo parameterInfo, int parameterLocation, ref object parameter);
	}

    /// <summary>
    /// This setter object takes a simple object array full of parameter data.
    /// It applys the objects in order to the method parameter list.
    /// </summary>
    public class OrderParameterSetter : IParameterSetter
	{
        private readonly object[] _paramList;
        private int _counter;

        public OrderParameterSetter(params object[] _parameters)
		{
			_paramList = _parameters;
		}

		public void Reset()
		{
			_counter = 0;
		}

		public bool GetParameterValue(ParameterInfo parameterInfo, int parameterLocation, ref object parameter)
		{
			if (_counter >= _paramList.Length)
            {
                return false;
            }

            parameter = _paramList[_counter++];
			return true;
		}
	}

	/// <summary>
	/// This setter object stores the parameter data in a Hashtable and uses the hashtable keys to match 
	/// the parameter names of the method to the parameter data.  This allows methods to be called like 
	/// stored procedures, with the parameters being passed in independent of order.
	/// </summary>
	public class NamedParameterSetter : IParameterSetter
	{
        private readonly Hashtable _parameters;

        public NamedParameterSetter(Hashtable parameters)
		{
			_parameters = parameters;
		}

		public void Reset()
		{

		}

		public bool GetParameterValue(ParameterInfo parameterInfo, int parameterLocation, ref object parameter)
		{
			string parameterName = parameterInfo.Name;

			if (!_parameters.ContainsKey(parameterName))
            {
                return false;
            }

            parameter = _parameters[parameterName];
			return true;
		}
	}

	/// <summary>
	/// ParameterSetterList maintains a collection of IParameterSetter objects and applies them in order to each
	/// parameter of the method.  Each time a match occurs the next parameter is tried starting with the first 
	/// setter object until it is matched.
	/// </summary>
	public class ParameterSetterList
	{
        private readonly ArrayList _list = new ArrayList();

        public void Add(IParameterSetter setter)
		{
			_list.Add(setter);
		}

		public IParameterSetter[] ToArray()
		{
			return (IParameterSetter[])_list.ToArray(typeof(IParameterSetter));
		}

		public void Reset()
		{
			foreach(IParameterSetter Setter in _list)
            {
                Setter.Reset();
            }
        }

		public object[] GetParameters(MethodInfo method)
		{
			ParameterInfo[] Params = method.GetParameters();

			object[] Values = new object[Params.Length];

			//TODO: Update to iterate backwards
			for(int i=0; i<Params.Length; ++i)
            {
                SetValue(Params[i], i, ref Values[i]);
            }

            return Values;
		}

		public object[] GetParameters(MethodInfo method, IParameterSetter lastSetter)
		{
			ParameterInfo[] Params = method.GetParameters();

			object[] Values = new object[Params.Length];

			//TODO: Update to iterate backwards
			for(int i=0; i<Params.Length; ++i)
			{
				if (!SetValue(Params[i], i, ref Values[i]))
                {
                    lastSetter.GetParameterValue(Params[i], i, ref Values[i]);
                }
            }

			return Values;
		}

		bool SetValue(ParameterInfo info, int i, ref object value)
		{
			foreach(IParameterSetter Setter in _list)
			{
				if (Setter.GetParameterValue(info, i, ref value))
                {
                    return true;
                }
            }

			return false;
		}
	}

	/// <summary>
	/// IMethodCall represents a partially specified parameter data list and a method.  This allows methods to be 
	/// dynamically late invoked for things like timers and other event driven frameworks.
	/// </summary>
	public interface IMethodCall
	{
		ParameterSetterList ParamList { get; }
		object Execute();
		object Execute(IParameterSetter Params);
		void EventHandler(object obj, EventArgs e);
		IAsyncResult BeginExecute(AsyncCallback callback, object obj);
		IAsyncResult  BeginExecute(IParameterSetter Params, AsyncCallback callback, object obj);
	}

	delegate object Exec();
	delegate object Exec2(IParameterSetter Params);

	/// <summary>
	/// Method call captures the data required to do a defered method call.
	/// </summary>
	public class DelegateMethodCall : MethodCallBase, IMethodCall
	{
		public DelegateMethodCall(Delegate f)
		{
            this.F = f;
		}

		public DelegateMethodCall(Delegate f, params object[] Params)
		{
			if (f.Method.GetParameters().Length < Params.Length)
            {
                throw new ArgumentException("Too many parameters specified for delegate", "f");
            }

            this.F = f;
			ParamList.Add(new OrderParameterSetter(Params));
		}

		public DelegateMethodCall(Delegate f, IParameterSetter Params)
		{
            this.F = f;
			ParamList.Add(Params);
		}

        public Delegate F { get; set; }

        public MethodInfo Method
		{
			get { return F.Method; }
		}

		public object Execute()
		{
			return F.DynamicInvoke(GetParameterList(Method));
		}

		public object Execute(IParameterSetter Params)
		{
			return F.DynamicInvoke(GetParameterList(Method, Params));
		}

		public void EventHandler(object obj, EventArgs e)
		{
			Execute();
		}

		private Exec _execute;

		public IAsyncResult BeginExecute(AsyncCallback callback, object obj)
		{
			_execute = new Exec(Execute);
			return _execute.BeginInvoke(callback, obj);
		}

		public IAsyncResult BeginExecute(IParameterSetter Params, AsyncCallback callback, object obj)
		{
			Exec2 exec = new Exec2(Execute);
			return exec.BeginInvoke(Params, callback, obj);
		}
	}

	public class DynamicMethodCall : MethodCallBase, IMethodCall
	{
		public DynamicMethodCall(MethodInfo method)
		{
			_obj = null;
			Method = method;
		}

		public DynamicMethodCall(object obj, MethodInfo method)
		{
			_obj = obj;
			Method = method;
		}

		public DynamicMethodCall(object obj, MethodInfo method, IParameterSetter setter)
		{
			_obj = obj;
			Method = method;
			ParamList.Add(setter);
		}

		private readonly object _obj;

        public MethodInfo Method { get; set; }

        public object Execute()
		{
			return Method.Invoke(_obj, GetParameterList(Method));
		}

		public object Execute(IParameterSetter Params)
		{
			return Method.Invoke(_obj, GetParameterList(Method, Params));
		}

		public void EventHandler(object obj, EventArgs e)
		{
			Execute();
		}

		Exec _exec;

		public IAsyncResult BeginExecute(AsyncCallback callback, object obj)
		{
			_exec = new Exec(Execute);
			return _exec.BeginInvoke(callback, null);
		}

		public IAsyncResult BeginExecute(IParameterSetter Params, AsyncCallback callback, object obj)
		{
			Exec2 exec = new Exec2(Execute);
			return exec.BeginInvoke(Params, callback, null);
		}
	}

	/// <summary>
	/// This is a base class that handles the Parameter list management for the 2 dynamic method call methods.
	/// </summary>
	public class MethodCallBase
	{
		private readonly ParameterSetterList _paramList = new ParameterSetterList();

		public ParameterSetterList ParamList
		{
			get { return _paramList; }
		}

		protected object[] GetParameterList(MethodInfo Method)
		{
			ParamList.Reset();
			object[] Params = ParamList.GetParameters(Method);
			return Params;
		}

		protected object[] GetParameterList(MethodInfo Method, IParameterSetter Params)
		{
			ParamList.Reset();
			object[] objParams = ParamList.GetParameters(Method, Params);
			return objParams;
		}
	}
}
