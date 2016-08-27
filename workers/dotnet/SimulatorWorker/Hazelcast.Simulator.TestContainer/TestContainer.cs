using System;
namespace Hazelcast.Simulator.TestContainer
{
	/**
	 * Container for test instances.
	 *
	 * It is responsible for:
	 * <ul>
	 * <li>Creates the test class instance by its fully qualified class name.</li>
	 * <li>Binding properties to the test class instance (test parameters).</li>
	 * <li>Injects required objects to annotated fields.</li>
	 * <li>Analyses the test class instance for annotated test phase methods.</li>
	 * <li>Provides a method to invoke test methods.</li>
	 * </ul>
	 */
	public class TestContainer
	{
		public TestContainer()
		{
		}
	}
}

