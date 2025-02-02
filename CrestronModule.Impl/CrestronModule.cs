﻿using System;
using System.Linq;

using Crestron.Logos.SplusObjects;
using CrestronModule.Core;

namespace CrestronModule.Impl
{
    public class CrestronModule : SplusObject, IModuleFactory, ICrestronLogger
    {
        ICrestronModule moduleImpl;
        uint stringParameterOffset = 10;

        public CrestronModule(
            string InstanceName,
            string ReferenceID,
            CrestronStringEncoding nEncodingType)
            : base(InstanceName, ReferenceID, nEncodingType)
        { 
        }

        public override void LogosSplusInitialize()
        {
            try
            {
                this.Trace($"CrestronModule LogosSplusInitialize {this.GetSymbolReferenceName()} {this.InstanceName} {this.GetSymbolInstanceName()}");
                var moduleType = this.GetType()
                    .Assembly.GetTypes()
                    .FirstOrDefault(t => typeof(ICrestronModule).IsAssignableFrom(t) && t.IsClass);

                if (moduleType != null)
                {
                    this.Trace("Module found: {0}", moduleType.Name);
                    this.moduleImpl = moduleType
                        .GetConstructor(new Type[] { typeof(IModuleFactory), typeof(ICrestronLogger) })
                        .Invoke(new object[] { this, this }) as ICrestronModule;
                }
            }
            catch (Exception ex) { this.ObjectCatchHandler(ex); }
            finally { this.ObjectFinallyHandler(); }
        }

        public override object FunctionMain(object __obj__)
        {
            try
            {
                var ctx = base.SplusFunctionMainStartCode();
                this.Trace("CrestronModule FunctionMain");

                var initializableModule = this.moduleImpl as IMainMethod;
                if (initializableModule != null) initializableModule.Main();
            }
            catch (Exception ex) { this.ObjectCatchHandler(ex); }
            finally { this.ObjectFinallyHandler(); }
            return __obj__;
        }

        protected override void ObjectCatchHandler(Exception e)
        {
            this.Trace("ObjectCatchHandler: {0}", e.Message);
            base.ObjectCatchHandler(e);
        }

        protected override void ObjectFinallyHandler()
        {
            this.Trace("ObjectFinallyHandler");
            base.ObjectFinallyHandler();
        }

        public IInput<bool> DigitalInput(string name, Action<bool> onChange)
        {
            var join = (uint)m_DigitalInputList.Count;
            var input = new Crestron.Logos.SplusObjects.DigitalInput(join, this);
            this.Trace("CreateDigitalInput {0}", join);
            m_DigitalInputList.Add(join, input);
            if (onChange != null) BindDigitalInput(input, onChange);
            return new DigitalInput(input);
        }
        public IOutput<bool> DigitalOutput(string name)
        {
            var join = (uint)m_DigitalOutputList.Count;
            var output = new Crestron.Logos.SplusObjects.DigitalOutput(join, this);
            this.Trace("CreateDigitalOutput {0}", join);
            m_DigitalOutputList.Add(join, output);
            return new DigitalOutput(output);
        }
        private void BindDigitalInput(Crestron.Logos.SplusObjects.DigitalInput input, Action<bool> onChange)
        {
            input.OnDigitalChange.Add(new InputChangeHandlerWrapper(o =>
            {
                this.Trace("OnDigitalChange");
                var e = o as SignalEventArgs;
                try
                {
                    var ctx = this.SplusThreadStartCode(e);
                    if (onChange != null) onChange(input.Value == 0 ? false : true);
                }
                catch (Exception ex) { this.ObjectCatchHandler(ex); }
                finally { this.ObjectFinallyHandler(e); }
                return this;
            }));
        }
        public IInput<string> StringInput(string name, int maxCapacity, Action<string> onChange)
        {
            var join = (uint)(m_AnalogInputList.Count + m_StringInputList.Count);
            var input = new Crestron.Logos.SplusObjects.StringInput(join, maxCapacity, this);
            this.Trace("CreateStringInput {0}", join);
            m_StringInputList.Add(join, input);
            if (onChange != null) BindStringInput(input, onChange);
            return new StringInput(input);
        }
        public IOutput<string> StringOutput(string name)
        {
            var join = (uint)(m_AnalogOutputList.Count + m_StringOutputList.Count);
            var output = new Crestron.Logos.SplusObjects.StringOutput(join, this);
            this.Trace("CreateStringOutput {0}", join);
            m_StringOutputList.Add(join, output);
            return new StringOutput(output);
        }
        private void BindStringInput(Crestron.Logos.SplusObjects.StringInput input, Action<string> onChange)
        {
            this.Trace("BindStringInput");
            input.OnSerialChange.Add(new InputChangeHandlerWrapper(o =>
            {
                this.Trace("OnSerialChange");
                var e = o as SignalEventArgs;
                try
                {
                    var ctx = this.SplusThreadStartCode(e);
                    if (onChange != null) onChange(input.Value.ToString());
                }
                catch (Exception ex) { this.ObjectCatchHandler(ex); }
                finally { this.ObjectFinallyHandler(e); }
                return this;
            }));
        }
        public IInput<ushort> AnalogInput(string name, Action<ushort> onChange)
        {
            var join = (uint)(m_AnalogInputList.Count + m_StringInputList.Count);
            var input = new Crestron.Logos.SplusObjects.AnalogInput(join, this);
            this.Trace("CreateAnalogInput {0}", join);
            m_AnalogInputList.Add(join, input);
            if (onChange != null) BindAnalogInput(input, onChange);
            return new AnalogInput(input);
        }
        public IOutput<ushort> AnalogOutput(string name)
        {
            var join = (uint)(m_AnalogOutputList.Count + m_StringOutputList.Count);
            var output = new Crestron.Logos.SplusObjects.AnalogOutput(join, this);
            this.Trace("CreateAnalogOutput {0}", join);
            m_AnalogOutputList.Add(join, output);
            return new AnalogOutput(output);
        }
        private void BindAnalogInput(Crestron.Logos.SplusObjects.AnalogInput input, Action<ushort> onChange)
        {
            this.Trace("BindAnalogInput");
            input.OnAnalogChange.Add(new InputChangeHandlerWrapper(o =>
            {
                this.Trace("OnAnalogChange");
                var e = o as SignalEventArgs;
                try
                {
                    var ctx = this.SplusThreadStartCode(e);
                    if (onChange != null) onChange(input.Value);
                }
                catch (Exception ex) { this.ObjectCatchHandler(ex); }
                finally { this.ObjectFinallyHandler(e); }
                return this;
            }));
        }

        public IParameter<string> StringParameter(string name, int maxCapacity)
        {
            var parameter = new Crestron.Logos.SplusObjects.StringParameter(this.stringParameterOffset, this);
            m_ParameterList.Add(this.stringParameterOffset, parameter);
            this.stringParameterOffset++;
            return new StringParameter(parameter);
        }
        void IModuleFactory.DigitalInputSkip()
        {
            // Do nothing
        }
        void IModuleFactory.DigitalOutputSkip()
        {
            // Do nothing
        }
        void IModuleFactory.StringInputSkip()
        {
            // Do nothing
        }
        void IModuleFactory.StringOutputSkip()
        {
            // Do nothing
        }
        void IModuleFactory.AnalogInputSkip()
        {
            // Do nothing
        }
        void IModuleFactory.AnalogOutputSkip()
        {
            // Do nothing
        }
        void IModuleFactory.StringParameterSkip()
        {
            // Do nothing
        }
    }
}
