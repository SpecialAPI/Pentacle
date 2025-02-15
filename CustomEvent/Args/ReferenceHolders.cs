using System;
using System.Collections.Generic;
using System.Text;

namespace Pentacle.CustomEvent.Args
{
    public interface IBoolReferenceHolder
    {
        BooleanReference BoolReference { get; }
    }

    public interface IIntReferenceHolder
    {
        IntegerReference IntReference { get; }
    }

    public interface IStringReferenceHolder
    {
        StringReference StringReference { get; }
    }

    public interface ITargetHolder
    {
        public IUnit Target { get; }
    }

    public abstract class BasicBoolReferenceHolder(BooleanReference boolRef) : IBoolReferenceHolder
    {
        public BooleanReference BoolReference => boolRef;
    }

    public abstract class BasicIntReferenceHolder(IntegerReference intRef) : IIntReferenceHolder
    {
        public IntegerReference IntReference => intRef;
    }

    public abstract class BasicStringReferenceHolder(StringReference stringRef) : IStringReferenceHolder
    {
        public StringReference StringReference => stringRef;
    }

    public abstract class BasicBoolAndIntReferenceHolder(BooleanReference boolRef, IntegerReference intRef) : IBoolReferenceHolder, IIntReferenceHolder
    {
        public BooleanReference BoolReference => boolRef;
        public IntegerReference IntReference => intRef;
    }

    public abstract class BasicIntAndStringReferenceHolder(IntegerReference intRef, StringReference stringRef) : IIntReferenceHolder, IStringReferenceHolder
    {
        public IntegerReference IntReference => intRef;
        public StringReference StringReference => stringRef;
    }

    public abstract class BasicBoolAndStringReferenceHolder(BooleanReference boolRef, StringReference stringRef) : IBoolReferenceHolder, IStringReferenceHolder
    {
        public BooleanReference BoolReference => boolRef;
        public StringReference StringReference => stringRef;
    }

    public abstract class BasicBoolAndIntAndStringReferenceHolder(BooleanReference boolRef, IntegerReference intRef, StringReference stringRef) : IBoolReferenceHolder, IIntReferenceHolder, IStringReferenceHolder
    {
        public BooleanReference BoolReference => boolRef;
        public IntegerReference IntReference => intRef;
        public StringReference StringReference => stringRef;
    }
}
