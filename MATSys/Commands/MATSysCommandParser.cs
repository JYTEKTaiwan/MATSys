using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MATSys.Commands
{
    internal class CommandLookUpTable : List<CommandItem>
    {
        public CommandItem this[string name] => this.FirstOrDefault(x => x.Alias == name);
        public CommandLookUpTable(MethodInfo[] methodInfos)
        {
            foreach (var item in methodInfos)
            {
                var att = item.GetCustomAttribute<MATSysCommandAttribute>();
                Type t;
                if (att.CommandType == null)
                {
                    t = GetCommandType(item);
                }
                else
                {
                    t = att.CommandType;
                }
                this.Add(new CommandItem(att.Alias, item, t));
            }
        }
        public bool IsExist(string name)
        {
            return this.Exists(x => x.Alias == name);
        }
        private Type GetCommandType(MethodInfo mi)
        {
            var types = mi.GetParameters().Select(x => x.ParameterType).ToArray();
            Type t = GetGenericCommandType(types.Length);
            if (t.IsGenericType)
            {
                return t.MakeGenericType(types);
            }
            else
            {
                return t;
            }

        }
        private Type GetGenericCommandType(int count)
        {
            switch (count)
            {
                case 0:
                    return typeof(Command);
                case 1:
                    return typeof(Command<>);
                case 2:
                    return typeof(Command<,>);
                case 3:
                    return typeof(Command<,,>);
                case 4:
                    return typeof(Command<,,,>);
                case 5:
                    return typeof(Command<,,,,>);
                case 6:
                    return typeof(Command<,,,,,>);
                case 7:
                    return typeof(Command<,,,,,,>);
                default:
                    return typeof(Command);
            }

        }


    }
    internal struct CommandItem
    {
        public string Alias { get; set; }
        public MethodInfo MethodInfo { get; set; }
        public Type CommandType { get; set; }

        public CommandItem(string alias, MethodInfo info, Type t)
        {
            Alias = alias;
            MethodInfo = info;
            CommandType = t;
        }
    }

}
