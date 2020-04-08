using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace Data
{
    public abstract class XmiElement
    {
        public virtual string XmiType() { return "xmi:"; }
        public string XmiId { get; set; }

        public XmiElement ParentXmiElement = null; // TODO
        public readonly LinkedList<XmiElement> ChildXmiElement = new LinkedList<XmiElement>(); // TODO
        //public abstract XmiElement ParentXmiElement(); // TODO
        //public abstract LinkedList<XmiElement> ChildrenXmiElements(); // TODO


        public XmiElement generateNewXmiId()
        {
            XmiId = Convert.ToBase64String(Encoding.UTF8.GetBytes(""
                + this.GetHashCode()
                + XmiType()
                + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss.fff", CultureInfo.InvariantCulture))); // TODO

            return this;
        }
    }
}
