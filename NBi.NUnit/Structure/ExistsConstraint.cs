﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NBi.Core.Analysis.Metadata;
using NBi.Core.Analysis.Metadata.Adomd;
using NBi.Core.Analysis.Request;
using NUnit.Framework.Constraints;
using NUnitCtr = NUnit.Framework.Constraints;

namespace NBi.NUnit.Structure
{
    public class ExistsConstraint : NUnitCtr.Constraint
    {

        protected IComparer comparer;
        protected MetadataDiscoveryRequest request;
        protected AdomdDiscoveryCommandFactory commandFactory;

        /// <summary>
        /// Engine dedicated to MetadataExtractor acquisition
        /// </summary>
        protected internal AdomdDiscoveryCommandFactory CommandFactory
        {
            set
            {
                if (value == null)
                    throw new ArgumentNullException();
                commandFactory = value;
            }
        }

        protected AdomdDiscoveryCommandFactory GetFactory()
        {
            if (commandFactory == null)
                commandFactory = new AdomdDiscoveryCommandFactory();
            return commandFactory;
        }

        /// <summary>
        /// Construct a ExistsConstraint
        /// </summary>
        public ExistsConstraint()
            : base()
        {

        }

        #region Modifiers
        /// <summary>
        /// Flag the constraint to ignore case and return self.
        /// </summary>
        public ExistsConstraint IgnoreCase
        {
            get
            {
                return this;
            }
        }

        #endregion

        public override bool Matches(object actual)
        {
            if (actual is MetadataDiscoveryRequest)
                return Process((MetadataDiscoveryRequest)actual);
            else if (actual is IEnumerable<IField>)
            {
                var res = doMatch((IEnumerable<IField>)actual);
                return res;
            }
            else
                throw new ArgumentException();
        }

        public bool doMatch(IEnumerable<IField> actual)
        {
            return (actual.Count() > 0);
        }


        protected bool Process(MetadataDiscoveryRequest actual)
        {
            request = actual;
            var factory = GetFactory();
            var command = factory.BuildExact(actual);
            IEnumerable<IField> structures = command.Execute();
            return this.Matches(structures);
        }

        protected void Investigate()
        {
            var factory = GetFactory();
            var command = factory.BuildExternal(request);
            IEnumerable<IField> structures = command.Execute();

            if (structures.Count() > 0)
            {
                this.actual = structures;
            }
        }

        /// <summary>
        /// Write a description of the constraint to a MessageWriter
        /// </summary>
        /// <param name="writer"></param>
        public override void WriteDescriptionTo(MessageWriter writer)
        {
            if (request != null)
            {
                var description = new DescriptionStructureHelper();
                var filterExpression = description.GetFilterExpression(request.GetAllFilters().Where(f => f.Target != request.Target));
                var notExpression = description.GetNotExpression(true);
                var targetExpression = description.GetTargetExpression(request.Target);
                var captionExpression = request.GetAllFilters().Single(f => f.Target==request.Target).Value;

                writer.WritePredicate(string.Format("find {0} {1} named '{2}' {3}"
                            , notExpression
                            , targetExpression
                            , captionExpression
                            , filterExpression));
            }
        }

        public override void WriteActualValueTo(MessageWriter writer)
        {
            Investigate();
            if (actual is IEnumerable<IField> && ((IEnumerable<IField>)actual).Count() > 0)
                base.WriteActualValueTo(writer);
            else
                writer.WriteActualValue(new NothingFoundMessage());
        }

        private class NothingFoundMessage
        {
            public override string ToString()
            {
                return "nothing found";
            }
        }
        
    }
}
