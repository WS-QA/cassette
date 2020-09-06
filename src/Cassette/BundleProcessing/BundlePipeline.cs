﻿using System;
using System.Collections.Generic;
using Cassette.TinyIoC;

namespace Cassette.BundleProcessing
{
    public class BundlePipeline<T> : List<IBundleProcessor<T>>, IBundlePipeline<T>
        where T : Bundle
    {
        readonly TinyIoCContainer container;

        protected BundlePipeline(TinyIoCContainer container)
        {
            this.container = container;
        }

        public void Add<TBundleProcessor>()
            where TBundleProcessor : class, IBundleProcessor<T>
        {
            var step = (IBundleProcessor<T>)container.Resolve<TBundleProcessor>();
            Add(step);
        }

        public void Add<TBundleProcessorFactory>(Func<TBundleProcessorFactory, IBundleProcessor<T>> create)
            where TBundleProcessorFactory : class
        {
            var step = create(container.Resolve<TBundleProcessorFactory>());
            Add(step);
        }

        public void Insert<TBundleProcessor>(int index)
            where TBundleProcessor : class, IBundleProcessor<T>
        {
            var step = (IBundleProcessor<T>)container.Resolve<TBundleProcessor>();
            Insert(index, step);
        }

        public void Insert<TBundleProcessorFactory>(int index, Func<TBundleProcessorFactory, IBundleProcessor<T>> create)
            where TBundleProcessorFactory : class
        {
            var step = create(container.Resolve<TBundleProcessorFactory>());
            Insert(index, step);
        }

        public void ReplaceWith<TReplacementProcessors>() where TReplacementProcessors : class, IEnumerable<IBundleProcessor<T>>
        {
            Clear();
            var replacementProcessors = container.Resolve<TReplacementProcessors>();
            AddRange(replacementProcessors);
        }

        public virtual void Process(T bundle)
        {
            foreach (var processor in this)
            {
                processor.Process(bundle);
            }
        }
    }
}