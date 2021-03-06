using SystemDot.Messaging.Diagnostics;
using SystemDot.Messaging.Distribution;
using SystemDot.Messaging.Ioc;
using SystemDot.Parallelism;

namespace SystemDot.Messaging.Pipelines
{
    public class ProcessorBuilder<TOut>
    {
        readonly IMessageProcessor<TOut> processor;

        public ProcessorBuilder(IMessageProcessor<TOut> processor)
        {
            this.processor = processor;
        }

        public ProcessorBuilder<TOut> Pump()
        {
            if (Debug.ShouldBuildSynchronousPipelines) return Pipe();

            var pump = new Pump<TOut>(GetTaskStarter());
            processor.MessageProcessed += pump.InputMessage;

            return new ProcessorBuilder<TOut>(pump);
        }

        public ProcessorBuilder<TOut> Queue()
        {
            if (Debug.ShouldBuildSynchronousPipelines) return Pipe();
 
            var queue = new Queue<TOut>(GetTaskStarter());
            processor.MessageProcessed += queue.InputMessage;
            
            return new ProcessorBuilder<TOut>(queue);
        }

        ProcessorBuilder<TOut> Pipe()
        {
            var pipe = new Pipe<TOut>();
            processor.MessageProcessed += pipe.InputMessage;

            return new ProcessorBuilder<TOut>(pipe);
        }

        static ITaskStarter GetTaskStarter()
        {
            return IocContainerLocator.Locate().Resolve<ITaskStarter>();
        }

        public ProcessorBuilder<TNextOut> ToConverter<TNextOut>(IMessageProcessor<TOut, TNextOut> messageProcessor)
        {
            processor.MessageProcessed += messageProcessor.InputMessage;
            return new ProcessorBuilder<TNextOut>(messageProcessor);
        }

        public ProcessorBuilder<TOut> ToProcessor(IMessageProcessor<TOut, TOut> messageProcessor)
        {
            processor.MessageProcessed += messageProcessor.InputMessage;
            return new ProcessorBuilder<TOut>(messageProcessor);
        }

        public ProcessorBuilder<TOut> ToProcessorIf(IMessageProcessor<TOut, TOut> messageProcessor, bool condition)
        {
            if (!condition) messageProcessor = new Pipe<TOut>();
            return ToProcessor(messageProcessor);
        }

        public void ToEndPoint(IMessageInputter<TOut> endPoint)
        {
            processor.MessageProcessed += endPoint.InputMessage;
        }

        
    }
}