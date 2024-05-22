using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FacturacionApi.Data.Sat.Catalogos
{
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1586.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.sat.gob.mx/cfd/3")]
    public partial class ComprobanteReceptor
    {

        private string rfcField;

        private string nombreField;

        private c_Pais residenciaFiscalField;

        private bool residenciaFiscalFieldSpecified;

        private string numRegIdTribField;

        private c_UsoCFDI usoCFDIField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Rfc
        {
            get
            {
                return this.rfcField;
            }
            set
            {
                this.rfcField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Nombre
        {
            get
            {
                return this.nombreField;
            }
            set
            {
                this.nombreField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public c_Pais ResidenciaFiscal
        {
            get
            {
                return this.residenciaFiscalField;
            }
            set
            {
                this.residenciaFiscalField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool ResidenciaFiscalSpecified
        {
            get
            {
                return this.residenciaFiscalFieldSpecified;
            }
            set
            {
                this.residenciaFiscalFieldSpecified = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string NumRegIdTrib
        {
            get
            {
                return this.numRegIdTribField;
            }
            set
            {
                this.numRegIdTribField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public c_UsoCFDI UsoCFDI
        {
            get
            {
                return this.usoCFDIField;
            }
            set
            {
                this.usoCFDIField = value;
            }
        }
    }

}
