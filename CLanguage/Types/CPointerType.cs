﻿using CLanguage.Interpreter;

namespace CLanguage.Types
{
    public class CPointerType : CType
    {
        public CType InnerType { get; private set; }

        public CPointerType(CType innerType)
        {
            InnerType = innerType;
        }

        public static readonly CPointerType PointerToConstChar = new CPointerType(CBasicType.ConstChar);

        public override int GetSize(EmitContext c)
        {
            return c.MachineInfo.PointerSize;
        }

        public override string ToString()
        {
            return "(Pointer " + InnerType + ")";
        }

        public override bool Equals (object obj)
        {
            return obj is CPointerType o && InnerType.Equals (o.InnerType);
        }

        public override int GetHashCode ()
        {
            int hash = 17;
            hash = hash * 37 + InnerType.GetHashCode ();
            hash = hash * 37 + 1;
            return hash;
        }
    }
}
