using AB15_GUI.WPF.Models;
using System.Threading.Tasks;

namespace AB15_GUI.Tests.Models
{
    /// <summary>
    /// Thread safe observable list tests. Initially used for logger
    /// </summary>
    /// <tc_links>
    ///     <link ID="ABEVBSW-64" Link="https://rb-tracker.bosch.com/tracker19/browse/ABEVBSW-64" />
    /// </tc_links>
    [TestFixture]
    [Parallelizable(scope: ParallelScope.Self)]
    public class ThreadSafeObservableList_Tests
    {
        /// <summary>
        /// Set up test environment
        /// </summary>
        [OneTimeSetUp]
        public void SetUp()
        {
        }

        /// <summary>
        /// Clean up environment after finishing tests
        /// </summary>
        [OneTimeTearDown]
        public void TearDown() 
        {
        }

        [TestCase("one", "two", "three"), Description("Basic operation test")]
        [TestCase("something1", "23", "else")]
        public void WhenProvidedCorrectInput_ThenStoredDataIsExpected(params string[] values)
        {
            // Arrange
            ThreadSafeObservableList<string> list = new ThreadSafeObservableList<string>();

            // Act
            foreach (string value in values)
            {
                list.Add(value);
            }

            // Assert
            // Number of records and values in records match expectation
            Assert.That(list.Count, Is.EqualTo(values.Length));
            for (int i = 0; i < values.Length; i++)
            {
                Assert.That(list[i], Is.EqualTo(values[i]));
            }
        }

        [TestCase(10), Description("Storage limit test")]
        [TestCase(100)]
        public void WhenStorageLimitExceeded_ThenCircularBufferLogicOperates(int listMaxItemsCount)
        {
            // Arrange
            ThreadSafeObservableList<string> list = new ThreadSafeObservableList<string>(listMaxItemsCount);

            // Act
            for (int i=0; i<listMaxItemsCount+2; i++)
            {
                list.Add($"some string {i}");
            }

            // Assert
            // Number of records and last value are expected
            Assert.That(list.Count, Is.EqualTo(listMaxItemsCount));
            Assert.That(list[listMaxItemsCount-1], Is.EqualTo($"some string {listMaxItemsCount + 1}"));
        }

        [Test, Description("Add method test")]
        public void WhenAddingValidItems_ThenExpectedNumberOfItemsAdded([Values(100, 50, 10)] int listMaxItemsCount, [Random(20, 30, 3)] int addedItemsCount)
        {
            // Arrange
            ThreadSafeObservableList<string> list = new ThreadSafeObservableList<string>(listMaxItemsCount);

            // Act
            for (int i = 0; i < addedItemsCount; i++)
            {
                list.Add($"some string {i}");
            }

            // Assert
            // Number of records expected
            if (addedItemsCount > listMaxItemsCount)
            {
                Assert.That(list.Count, Is.EqualTo(listMaxItemsCount));
            }
            else
            {
                Assert.That(list.Count, Is.EqualTo(addedItemsCount));
            }
        }

        [Test, Description("Remove method test")]
        public void WhenRemovingItems_ThenExpectedNumberOfItemsRemoved([Random(5, 10, 5)] int removedItemsCount)
        {
            // Arrange
            bool isRemovedSuccessfully = true;
            int initialItemsCount = 15;
            ThreadSafeObservableList<string> list = new ThreadSafeObservableList<string>();
            for (int i = 0; i < initialItemsCount; i++)
            {
                list.Add($"some string {i}");
            }

            // Act
            string tmpItem;
            for (int i = 0; i < removedItemsCount; i++)
            {
                tmpItem = list[initialItemsCount - i - 1];
                isRemovedSuccessfully &= list.Remove(tmpItem);
            }


            // Assert
            // All Remove calls were successful
            Assert.That(isRemovedSuccessfully);
            
            // Count of items after removal is expected
            Assert.That(list.Count, Is.EqualTo(initialItemsCount - removedItemsCount));
        }

        [Test, Description("Indexer value modification test")]
        public void WhenModifyingItemsWithIndexer_ThenItemsAreExpected()
        {
            // Arrange
            string expectedNewValue = "new value";
            ThreadSafeObservableList<string> list = ["something 1", "Something 2", "Something 3"];


            // Act
            list[1] = expectedNewValue;


            // Assert
            // Value was modified successfuly
            Assert.That(list[1], Is.EqualTo(expectedNewValue));
        }

        [Test, Description("Clear method test")]
        public void WhenClearing_ThenAllItemsRemoved()
        {
            // Arrange
            ThreadSafeObservableList<string> list = ["something 1", "Something 2", "Something 3"];


            // Act
            list.Clear();


            // Assert
            // List is empty
            Assert.That(list.Count, Is.EqualTo(0));
        }

        [Test, Description("Multithreading test")]
        public void WhenWritingFromFewThreads_ThenAllItemsAreAdded([Random(20, 200, 5)] int itemsCount)
        {
            // Arrange 
            ThreadSafeObservableList<string> list = new ThreadSafeObservableList<string>(700);

            // Act
            Task taskHandle1 = Task.Run(() => 
            {
                for (int i = 0; i < itemsCount; i++)
                {
                    list.Add($"1 thread {i}");
                }
            });

            Task taskHandle2 = Task.Run(() =>
            {
                for (int i = 0; i < itemsCount; i++)
                {
                    list.Add($"2 thread {i}");
                }
            });

            Task taskHandle3 = Task.Run(() =>
            {
                for (int i = 0; i < itemsCount; i++)
                {
                    list.Add($"3 thread {i}");
                }
            });

            Task.WaitAll(taskHandle1, taskHandle2 , taskHandle3);

            // Assert
            // Expected number of items were added
            Assert.That(list.Count, Is.EqualTo(itemsCount*3));
        }

        [Test, Description("CollectionChanged event raising test")]
        public void WhenListChanges_ThenCollectionChangedEventIsRaised()
        {
            // Arrange 
            int numberOfCalls = 0;
            ThreadSafeObservableList<string> list = new ThreadSafeObservableList<string>();
            for (int i = 0; i < 5; i++)
            {
                list.Add($"some string {i}");
            }
            list.CollectionChanged += (s, e) => { numberOfCalls++; };

            // Act
            var tmpItem = list.IndexOf("some string 2");

            list.Remove(list[0]);
            list.RemoveAt(0);
            list.Add($"some string");
            list[0] = "new data";
            list.Clear();

            // Assert
            // Number of calls to CollectionChanged event mathces number of list modifications in test
            Assert.That(numberOfCalls, Is.EqualTo(5));
        }

        [Test, Description("PropertyChanged event raising test")]
        public void WhenPropertiesChanges_ThenPropertyChangedEventIsRaised()
        {
            // Arrange 
            int numberOfCalls = 0;
            ThreadSafeObservableList<string> list = new ThreadSafeObservableList<string>();
            for (int i = 0; i < 5; i++)
            {
                list.Add($"some string {i}");
            }
            list.PropertyChanged += (s, e) => { numberOfCalls++; };

            // Act
            list[0] = "new data";

            list.Remove(list[0]);
            list.RemoveAt(0);
            list.Add($"some string");
            list.Clear();

            // Assert
            // Number of calls to PropertyChanged event mathces number of properties modifications in test
            Assert.That(numberOfCalls, Is.EqualTo(4));
        }
    }
}