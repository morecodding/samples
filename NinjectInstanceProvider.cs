public class NinjectInstanceProvider : IInstanceProvider
    {
        private Type serviceType;
        private IKernel kernel;

        public NinjectInstanceProvider(Type serviceType, IKernel kernel)
        {
            this.kernel = kernel;
            this.serviceType = serviceType;
        }

        public object GetInstance(InstanceContext instanceContext, Message message)
        {
            return kernel.Get(this.serviceType);
        }

        public object GetInstance(InstanceContext instanceContext)
        {
            return this.GetInstance(instanceContext, null);
        }

        public void ReleaseInstance(InstanceContext instanceContext, object instance)
        {
        }
    }

    public class NinjectBehaviorAttribute : Attribute, IServiceBehavior
    {

        public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, System.Collections.ObjectModel.Collection<ServiceEndpoint> endpoints, BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            Type serviceType = serviceDescription.ServiceType;
            IInstanceProvider instanceProvider = new NinjectInstanceProvider(serviceType, new StandardKernel(new MyModule()));

            foreach (ChannelDispatcher dispatcher in serviceHostBase.ChannelDispatchers)
            {
                foreach (EndpointDispatcher endpointDispatcher in dispatcher.Endpoints)
                {
                    DispatchRuntime dispatchRuntime = endpointDispatcher.DispatchRuntime;
                    dispatchRuntime.InstanceProvider = instanceProvider;
                }
            }
        }

        public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {

        }
    }

    public class NinjectBehaviorExtensionElement : BehaviorExtensionElement
    {
        public override Type BehaviorType
        {
            get { return typeof(NinjectBehaviorAttribute); }
        }

        protected override object CreateBehavior()
        {
            return new NinjectBehaviorAttribute();
        }
    }

    public class MyModule : NinjectModule
    {

        public override void Load()
        {
            this.Bind<IFaturaRepositoryService>().To<MockFaturaRepositoryService>().InTransientScope();
        }
    }
