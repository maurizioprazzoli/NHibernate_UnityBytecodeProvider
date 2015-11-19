using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.InterceptionExtension;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model;
using NHibernate;
using NHibernate_UnityBytecodeProvider;
using System;
using System.Collections.Generic;
using Unity.Bytecode;

namespace TestInterceptionLeazyLoading
{
    public abstract class BaseUnityInterception
    {
        // Define unity repository
        protected IUnityContainer container;
        protected ISessionFactory sessionFactory;

        public BaseUnityInterception()
        {
            // Set Up new container
            container = new UnityContainer();

            container.RegisterType<Item, Item>(new InterceptionBehavior<PolicyInjectionBehavior>(),
                                               new Interceptor<VirtualMethodInterceptor>(),
                                               new AdditionalInterface(typeof(IIntercepted<Item>)));
            // Add interception component
            container.AddNewExtension<Interception>();

            // RegisterUnitOfwork
            Dictionary<string, string> configuration = new Dictionary<string, string>();

            ComposeConfiguration(configuration);

            sessionFactory = NHConfiguration.ConfigureNHibernate(configuration, container);
        }

         [TestMethod]
        public virtual void ObjectGeneratedFromContainerCanBeInsertedAndRetrievedFromDatabaseWithExceptionForLeazyLoading()
        {
            Guid item_guid = Guid.NewGuid();
            Guid bid1_guid = Guid.NewGuid();
            Guid bid2_guid = Guid.NewGuid();

            Item item = container.Resolve<Item>();
            item.Id = item_guid;
            item.Description = "Item Description";

            item.AddBid(bid1_guid, "Bid1 Description");
            item.AddBid(bid2_guid, "Bid2 Description");

            Assert.IsTrue(item.Id == item_guid);
            Assert.IsTrue(item.Description == "Item Description");

            Assert.IsTrue(item.Bids[0].Id == bid1_guid);
            Assert.IsTrue(item.Bids[0].Description == "Bid1 Description");

            Assert.IsTrue(item.Bids[1].Id == bid2_guid);
            Assert.IsTrue(item.Bids[1].Description == "Bid2 Description");

            using (var session = sessionFactory.OpenSession())
            {
                using (var tx = session.BeginTransaction())
                {
                    session.Save(item);
                    tx.Commit();
                }
            }

            StaticCounterHelper.ResetCounter();

            item.TestInterceptedMethodWithSuccess();

            try
            {
                item.TestInterceptedMethodWithException();
            }
            catch
            { }

            Assert.IsTrue(StaticCounterHelper.SuccessCounter == 1);
            Assert.IsTrue(StaticCounterHelper.ExceptionCounter == 1);

            Item itemRetrieved;
            using (var session = sessionFactory.OpenSession())
            {
                using (var tx = session.BeginTransaction())
                {
                    itemRetrieved = session.Get<Item>(item_guid);
                    tx.Commit();
                }
            }

            Int32 numberOfException = 0;
            try
            {
                Assert.IsTrue(itemRetrieved.Id == item_guid);
            }
            catch
            {
                numberOfException++;
            }

            try
            {
                Assert.IsTrue(itemRetrieved.Description == "Item Description");
            }
            catch
            {
                numberOfException++;
            }

            try
            {
                foreach (Bid bid in itemRetrieved.Bids)
                {
                    Assert.IsTrue(bid.Id != default(Guid));
                    Assert.IsTrue(bid.Description != String.Empty);

                }

            }
            catch
            {
                numberOfException++;
            }

            Assert.IsTrue(numberOfException == 1);

            StaticCounterHelper.ResetCounter();

            itemRetrieved.TestInterceptedMethodWithSuccess();

            try
            {
                itemRetrieved.TestInterceptedMethodWithException();
            }
            catch
            { }

            Assert.IsTrue(StaticCounterHelper.SuccessCounter == 1);
            Assert.IsTrue(StaticCounterHelper.ExceptionCounter == 1);
        }

        public abstract void ComposeConfiguration(Dictionary<string, string> configuration);
    }
}
