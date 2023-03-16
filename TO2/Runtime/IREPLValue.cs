using System;
using KontrolSystem.Parsing;
using KontrolSystem.TO2.AST;

namespace KontrolSystem.TO2.Runtime {
    public abstract class REPLValueFuture : Future<IREPLValue> {
        private readonly TO2Type to2Type;

        protected REPLValueFuture(TO2Type to2Type) {
            this.to2Type = to2Type;
        }

        public TO2Type Type => to2Type;
        
        public class Success : REPLValueFuture {
            private readonly IREPLValue value;

            public Success(IREPLValue value) : base(value.Type) => this.value = value;

            public override FutureResult<IREPLValue> PollValue() => new FutureResult<IREPLValue>(value);
        }
    }

    public interface IREPLValue {
        TO2Type Type { get; }
        
        object Value { get; }
    }

    public struct REPLBool : IREPLValue {
        public readonly bool boolValue;
        
        public REPLBool( bool boolValue) {
            this.boolValue = boolValue;
        }

        public TO2Type Type => BuiltinType.Bool;
        
        public object Value => boolValue;

        public static IREPLValue Not(Node node, IREPLValue other, IREPLValue _) {
            if(other is REPLBool b) {
                return new REPLBool(!b.boolValue);
            }

            throw new REPLException(node, $"Can not preform boolean not on non-boolean: {other.Type.Name}");
        }

        public static IREPLValue Eq(Node node, IREPLValue left, IREPLValue right) {
            if(left is REPLBool lb && right is REPLBool rb) {
                return new REPLBool(lb.boolValue == rb.boolValue);
            }

            throw new REPLException(node, $"Can not preform boolean eq on non-boolean: {left.Type.Name} {right.Type.Name}");
        }
        
        public static IREPLValue Neq(Node node, IREPLValue left, IREPLValue right) {
            if(left is REPLBool lb && right is REPLBool rb) {
                return new REPLBool(lb.boolValue != rb.boolValue);
            }

            throw new REPLException(node, $"Can not preform boolean neq on non-boolean: {left.Type.Name} {right.Type.Name}");
        }
        
        public static IREPLValue And(Node node, IREPLValue left, IREPLValue right) {
            if(left is REPLBool lb && right is REPLBool rb) {
                return new REPLBool(lb.boolValue && rb.boolValue);
            }

            throw new REPLException(node, $"Can not preform boolean and on non-boolean: {left.Type.Name} {right.Type.Name}");
        }
        
        public static IREPLValue Or(Node node, IREPLValue left, IREPLValue right) {
            if(left is REPLBool lb && right is REPLBool rb) {
                return new REPLBool(lb.boolValue || rb.boolValue);
            }

            throw new REPLException(node, $"Can not preform boolean or on non-boolean: {left.Type.Name} {right.Type.Name}");
        }
    }
    
    public struct REPLInt : IREPLValue {
        public readonly long intValue;
        
        public REPLInt(long intValue) {
            this.intValue = intValue;
        }

        public TO2Type Type => BuiltinType.Int;
        
        public object Value => intValue;
        
        public static IREPLValue Neg(Node node, IREPLValue other, IREPLValue _) {
            if(other is REPLInt i) {
                return new REPLInt(-i.intValue);
            }

            throw new REPLException(node, $"Can not preform int neg on non-int: {other.Type.Name}");
        }
        
        public static IREPLValue Add(Node node, IREPLValue left, IREPLValue right) {
            if(left is REPLInt li && right is REPLInt ri) {
                return new REPLInt(li.intValue + ri.intValue);
            }

            throw new REPLException(node, $"Can not preform int add on non-int: {left.Type.Name} {right.Type.Name}");
        }
        
        public static IREPLValue Sub(Node node, IREPLValue left, IREPLValue right) {
            if(left is REPLInt li && right is REPLInt ri) {
                return new REPLInt(li.intValue - ri.intValue);
            }

            throw new REPLException(node, $"Can not preform int sub on non-int: {left.Type.Name} {right.Type.Name}");
        }

        public static IREPLValue Mul(Node node, IREPLValue left, IREPLValue right) {
            if(left is REPLInt li && right is REPLInt ri) {
                return new REPLInt(li.intValue * ri.intValue);
            }

            throw new REPLException(node, $"Can not preform int mul on non-int: {left.Type.Name} {right.Type.Name}");
        }
        
        public static IREPLValue Div(Node node, IREPLValue left, IREPLValue right) {
            if(left is REPLInt li && right is REPLInt ri) {
                return new REPLInt(li.intValue / ri.intValue);
            }

            throw new REPLException(node, $"Can not preform int div on non-int: {left.Type.Name} {right.Type.Name}");
        }
        
        public static IREPLValue Rem(Node node, IREPLValue left, IREPLValue right) {
            if(left is REPLInt li && right is REPLInt ri) {
                return new REPLInt(li.intValue % ri.intValue);
            }

            throw new REPLException(node, $"Can not preform int rem on non-int: {left.Type.Name} {right.Type.Name}");
        }

        public static IREPLValue BitOr(Node node, IREPLValue left, IREPLValue right) {
            if(left is REPLInt li && right is REPLInt ri) {
                return new REPLInt(li.intValue | ri.intValue);
            }

            throw new REPLException(node, $"Can not preform int bit or on non-int: {left.Type.Name} {right.Type.Name}");
        }

        public static IREPLValue BitAnd(Node node, IREPLValue left, IREPLValue right) {
            if(left is REPLInt li && right is REPLInt ri) {
                return new REPLInt(li.intValue & ri.intValue);
            }

            throw new REPLException(node, $"Can not preform int bit and on non-int: {left.Type.Name} {right.Type.Name}");
        }
        
        public static IREPLValue BitXor(Node node, IREPLValue left, IREPLValue right) {
            if(left is REPLInt li && right is REPLInt ri) {
                return new REPLInt(li.intValue ^ ri.intValue);
            }

            throw new REPLException(node, $"Can not preform int bit xor on non-int: {left.Type.Name} {right.Type.Name}");
        }

        public static IREPLValue Eq(Node node, IREPLValue left, IREPLValue right) {
            if(left is REPLInt li && right is REPLInt ri) {
                return new REPLBool(li.intValue == ri.intValue);
            }

            throw new REPLException(node, $"Can not preform int eq on non-int: {left.Type.Name} {right.Type.Name}");
        }
        
        public static IREPLValue Neq(Node node, IREPLValue left, IREPLValue right) {
            if(left is REPLInt li && right is REPLInt ri) {
                return new REPLBool(li.intValue != ri.intValue);
            }

            throw new REPLException(node, $"Can not preform int neq on non-int: {left.Type.Name} {right.Type.Name}");
        }

        public static IREPLValue Gt(Node node, IREPLValue left, IREPLValue right) {
            if(left is REPLInt li && right is REPLInt ri) {
                return new REPLBool(li.intValue > ri.intValue);
            }

            throw new REPLException(node, $"Can not preform int gt on non-int: {left.Type.Name} {right.Type.Name}");
        }
        
        public static IREPLValue Geq(Node node, IREPLValue left, IREPLValue right) {
            if(left is REPLInt li && right is REPLInt ri) {
                return new REPLBool(li.intValue >= ri.intValue);
            }

            throw new REPLException(node, $"Can not preform int geq on non-int: {left.Type.Name} {right.Type.Name}");
        }        

        public static IREPLValue Lt(Node node, IREPLValue left, IREPLValue right) {
            if(left is REPLInt li && right is REPLInt ri) {
                return new REPLBool(li.intValue < ri.intValue);
            }

            throw new REPLException(node, $"Can not preform int lt on non-int: {left.Type.Name} {right.Type.Name}");
        }
        
        public static IREPLValue Leq(Node node, IREPLValue left, IREPLValue right) {
            if(left is REPLInt li && right is REPLInt ri) {
                return new REPLBool(li.intValue <= ri.intValue);
            }

            throw new REPLException(node, $"Can not preform int leq on non-int: {left.Type.Name} {right.Type.Name}");
        }        
    }

    public struct REPLFloat : IREPLValue {
        public readonly double floatValue;
        
        public REPLFloat(double floatValue) {
            this.floatValue = floatValue;
        }

        public TO2Type Type => BuiltinType.Float;
        
        public object Value => floatValue;
        
        public static IREPLValue Neg(Node node, IREPLValue other, IREPLValue _) {
            if(other is REPLFloat f) {
                return new REPLFloat(-f.floatValue);
            }

            throw new REPLException(node, $"Can not preform float neg on non-float: {other.Type.Name}");
        }
        
        public static IREPLValue Add(Node node, IREPLValue left, IREPLValue right) {
            if(left is REPLFloat lf && right is REPLFloat rf) {
                return new REPLFloat(lf.floatValue + rf.floatValue);
            }

            throw new REPLException(node, $"Can not preform int add on non-int: {left.Type.Name} {right.Type.Name}");
        }
        
        public static IREPLValue Sub(Node node, IREPLValue left, IREPLValue right) {
            if(left is REPLFloat lf && right is REPLFloat rf) {
                return new REPLFloat(lf.floatValue - rf.floatValue);
            }

            throw new REPLException(node, $"Can not preform int sub on non-int: {left.Type.Name} {right.Type.Name}");
        }

        public static IREPLValue Mul(Node node, IREPLValue left, IREPLValue right) {
            if(left is REPLFloat lf && right is REPLFloat rf) {
                return new REPLFloat(lf.floatValue * rf.floatValue);
            }

            throw new REPLException(node, $"Can not preform int mul on non-int: {left.Type.Name} {right.Type.Name}");
        }
        
        public static IREPLValue Div(Node node, IREPLValue left, IREPLValue right) {
            if(left is REPLFloat lf && right is REPLFloat rf) {
                return new REPLFloat(lf.floatValue / rf.floatValue);
            }

            throw new REPLException(node, $"Can not preform int div on non-int: {left.Type.Name} {right.Type.Name}");
        }
        
        public static IREPLValue Rem(Node node, IREPLValue left, IREPLValue right) {
            if(left is REPLFloat lf && right is REPLFloat rf) {
                return new REPLFloat(lf.floatValue % rf.floatValue);
            }

            throw new REPLException(node, $"Can not preform int rem on non-int: {left.Type.Name} {right.Type.Name}");
        }

        public static IREPLValue Eq(Node node, IREPLValue left, IREPLValue right) {
            if(left is REPLFloat lf && right is REPLFloat rf) {
                return new REPLBool(lf.floatValue == rf.floatValue);
            }

            throw new REPLException(node, $"Can not preform int eq on non-int: {left.Type.Name} {right.Type.Name}");
        }
        
        public static IREPLValue Neq(Node node, IREPLValue left, IREPLValue right) {
            if(left is REPLFloat lf && right is REPLFloat rf) {
                return new REPLBool(lf.floatValue != rf.floatValue);
            }

            throw new REPLException(node, $"Can not preform int neq on non-int: {left.Type.Name} {right.Type.Name}");
        }

        public static IREPLValue Gt(Node node, IREPLValue left, IREPLValue right) {
            if(left is REPLFloat lf && right is REPLFloat rf) {
                return new REPLBool(lf.floatValue > rf.floatValue);
            }

            throw new REPLException(node, $"Can not preform int gt on non-int: {left.Type.Name} {right.Type.Name}");
        }
        
        public static IREPLValue Geq(Node node, IREPLValue left, IREPLValue right) {
            if(left is REPLFloat lf && right is REPLFloat rf) {
                return new REPLBool(lf.floatValue >= rf.floatValue);
            }

            throw new REPLException(node, $"Can not preform int geq on non-int: {left.Type.Name} {right.Type.Name}");
        }        

        public static IREPLValue Lt(Node node, IREPLValue left, IREPLValue right) {
            if(left is REPLFloat lf && right is REPLFloat rf) {
                return new REPLBool(lf.floatValue < rf.floatValue);
            }

            throw new REPLException(node, $"Can not preform int lt on non-int: {left.Type.Name} {right.Type.Name}");
        }
        
        public static IREPLValue Leq(Node node, IREPLValue left, IREPLValue right) {
            if(left is REPLFloat lf && right is REPLFloat rf) {
                return new REPLBool(lf.floatValue <= rf.floatValue);
            }

            throw new REPLException(node, $"Can not preform int leq on non-int: {left.Type.Name} {right.Type.Name}");
        }        
    }
    
    public struct REPLString : IREPLValue {
        public readonly string stringValue;
        
        public REPLString(string stringValue) {
            this.stringValue = stringValue;
        }

        public TO2Type Type => BuiltinType.Float;
        
        public object Value => stringValue;
    }

    public struct REPLAny : IREPLValue {
        public readonly TO2Type type;
        public readonly object anyValue;

        public REPLAny(TO2Type type, object anyValue) {
            this.type = type;
            this.anyValue = anyValue;
        }
        
        public TO2Type Type => type;
        
        public object Value => anyValue;
    }
}
