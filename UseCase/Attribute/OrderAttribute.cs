using System;
namespace UseCase.Attribute
{
    public class OrderAttribute : System.Attribute
    {
        public byte order;

        public OrderAttribute(byte order)
        {
            this.order = order;
        }
    }
}
