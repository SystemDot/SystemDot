using System;
using SystemDot.Messaging.Packaging;
using SystemDot.Messaging.Pipelines;
using SystemDot.Messaging.Storage;
using SystemDot.Parallelism;

namespace SystemDot.Messaging.Repeating
{
    public static class RepeaterMessagePipelineBuilderExtensions
    {
        public static ProcessorBuilder<MessagePayload> ToSimpleMessageRepeater(
            this ProcessorBuilder<MessagePayload> builder,
            MessageCache messageCache,
            ICurrentDateProvider currentDateProvider,
            ITaskRepeater taskRepeater)
        {
            return builder.ToMessageRepeater(messageCache, currentDateProvider, taskRepeater, new SimpleRepeatStrategy());
        }

        public static ProcessorBuilder<MessagePayload> ToMessageRepeater(
            this ProcessorBuilder<MessagePayload> builder, 
            MessageCache messageCache, 
            ICurrentDateProvider currentDateProvider, 
            ITaskRepeater taskRepeater, 
            IRepeatStrategy strategy)
        {
            var repeater = new MessageRepeater(strategy, currentDateProvider, messageCache);
            taskRepeater.Register(TimeSpan.FromSeconds(1), repeater.Start);

            return builder.ToProcessor(repeater);
        }
    }
}