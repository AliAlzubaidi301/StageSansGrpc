using CodeExceptionManager.Model.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Orthodyne.CoreCommunicationLayer.Models.Common
{
    public class GenericProperty : IConvertible
    {
        private List<object> authorizedValues = new List<object>();
        public string Name { get; set; }
        public Type Type { get; set; }
        public object Value { get; set; }
        public List<object> AuthorizedValues { get { return authorizedValues; } }
        public string DisplayedName { get; set; }
        public string ProxyOption { get; set; }

        public void UpdateValue(object value)
        {
            try
            {
                value = Convert.ChangeType(value, Type);
                if (this.AuthorizedValues.Count() > 0) {
                    if (!this.AuthorizedValues.Contains(value)) throw new ArgumentException("Not authorized value (" + value.ToString() + ") provided for property : " + Name);                   
                }
                this.Value = value;
            }
            catch (Exception ex)
            {
                new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        public GenericProperty CopyItem()
        {
            GenericProperty copy = new GenericProperty()
            {
                Name = this.Name,
                Type = this.Type,
                Value = this.Value,
                DisplayedName = this.DisplayedName,
                ProxyOption = this.ProxyOption
            };
            copy.AuthorizedValues.AddRange(this.authorizedValues);
            return copy;
        }
        public TypeCode GetTypeCode()
        {
            return Type.GetTypeCode(Type);
        }

        public bool ToBoolean(IFormatProvider provider)
        {
            if (Value != null && GetTypeCode() == TypeCode.Boolean)
                return Convert.ToBoolean(Value);
            else return false;
        }

        public byte ToByte(IFormatProvider provider)
        {
            if (Value != null && GetTypeCode() == TypeCode.Byte)
                return Convert.ToByte(Value);
            else return 0;
        }

        public char ToChar(IFormatProvider provider)
        {
            if (Value != null && GetTypeCode() == TypeCode.Char)
                return Convert.ToChar(Value);
            else return ' ';
        }

        public DateTime ToDateTime(IFormatProvider provider)
        {
            if (Value != null && GetTypeCode() == TypeCode.DateTime)
                return Convert.ToDateTime(Value);
            else return DateTime.Now;
        }

        public decimal ToDecimal(IFormatProvider provider)
        {
            if (Value != null && GetTypeCode() == TypeCode.Decimal)
                return Convert.ToDecimal(Value);
            else return 0;
        }

        public double ToDouble(IFormatProvider provider)
        {
            if (Value != null && GetTypeCode() == TypeCode.Double)
                return Convert.ToDouble(Value);
            else return -1;
        }

        public short ToInt16(IFormatProvider provider)
        {
            if (Value != null && GetTypeCode() == TypeCode.Int16)
                return Convert.ToInt16(Value);
            else return -1;
        }

        public int ToInt32(IFormatProvider provider)
        {
            if (Value != null && GetTypeCode() == TypeCode.Int32)
                return Convert.ToInt32(Value);
            else return -1;
        }

        public long ToInt64(IFormatProvider provider)
        {
            if (Value != null && GetTypeCode() == TypeCode.Int64)
                return Convert.ToInt64(Value);
            else return -1;
        }

        public sbyte ToSByte(IFormatProvider provider)
        {
            if (Value != null && GetTypeCode() == TypeCode.SByte)
                return Convert.ToSByte(Value);
            else return -1;
        }

        public float ToSingle(IFormatProvider provider)
        {
            if (Value != null && GetTypeCode() == TypeCode.Single)
                return Convert.ToSingle(Value);
            else return -1;
        }

        public string ToString(IFormatProvider provider)
        {
            if (Value != null)
                return Value.ToString();
            else return "";
        }

        public object ToType(Type conversionType, IFormatProvider provider)
        {
            if (Value != null) return Convert.ChangeType(Value, conversionType);
            else return null;
        }

        public ushort ToUInt16(IFormatProvider provider)
        {
            if (Value != null && GetTypeCode() == TypeCode.UInt16)
                return Convert.ToUInt16(Value);
            else return 0;
        }

        public uint ToUInt32(IFormatProvider provider)
        {
            if (Value != null && GetTypeCode() == TypeCode.UInt32)
                return Convert.ToUInt32(Value);
            else return 0;
        }

        public ulong ToUInt64(IFormatProvider provider)
        {
            if (Value != null && GetTypeCode() == TypeCode.UInt64)
                return Convert.ToUInt64(Value);
            else return 0;
        }

    }
}
