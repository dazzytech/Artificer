using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

/// <summary>
/// Base component used by objects
/// with the intended use of
/// being indexed within a list
/// </summary>
[System.Serializable]
public class IndexedObject
{
    public int ID;
}

/// <summary>
/// Container for list of objects
/// that are stored and accessed using
/// ID instead of an index
/// </summary>
public class IndexedList<T>: List<T> where T: IndexedObject
{
    #region ID MANAGER

    /// <summary>
    /// An embedded class used for the management of IDs within class
    /// </summary>
    public class IDManagerClass
    {
        // Keep a reference to existing IDs within container
        private List<int> m_IDList;

	    // Use this for initialization
	    public IDManagerClass()
        {
            m_IDList = new List<int>();
	    }

        /// <summary>
        /// Returns the next available ID
        /// and stores it
        /// </summary>
        /// <returns></returns>
        public int Next()
        {
            // starting value to be interated across list
            int current = 0;

            // sort list ascending numerically
            m_IDList.Sort();

            // With each element in numeric order
            // we can test if there is a gap
            // between ID e.g if an element was deleted
            foreach(int item in m_IDList)
            {
                if(current < item)
                {
                    // if current is less
                    // then we have found a gap in the list
                    break;
                }

                current++;
            }

            m_IDList.Add(current);

            return current;
        }

        public void Delete(int ID)
        {
            // Find our ID within list and remove it
            for(int i = 0; i < m_IDList.Count; i++)
            {
                if(ID == m_IDList[i])
                {
                    m_IDList.RemoveAt(i);
                }
            }
        }

    }

    #endregion

    private IDManagerClass m_IDManager;

    public IndexedList()
    {
        m_IDManager = new IDManagerClass();
    }

    /// <summary>
    /// Creates a new 
    /// </summary>
    /// <param name="newItem"></param>
    new public void Add(T newItem) 
    {
        // Return next available ID with IDManager
        newItem.ID = m_IDManager.Next();

        base.Add(newItem);
    }

    new public void Remove(T obj)
    {
        if(base.Remove(obj))
        {
            m_IDManager.Delete(obj.ID);
        }
    }

    /// <summary>
    /// Deletes object from list using
    /// its ID
    /// </summary>
    /// <param name="ID"></param>
    public void Remove(int ID)
    {
        // Find our ID within list and remove it
        for (int i = 0; i < base.Count; i++)
        {
            if (ID == ((IndexedObject)base[i]).ID)
            {
                m_IDManager.Delete(((IndexedObject)base[i]).ID);
                base.RemoveAt(i);
                break;
            }
        }
    }

    /// <summary>
    /// Use ID to retrieve ID
    /// </summary>
    /// <param name="ID"></param>
    /// <returns></returns>
    public T Item(int ID)
    {
        // Find our ID within list and remove it
        for (int i = 0; i < base.Count; i++)
        {
            if (ID == ((T)base[i]).ID)
            {
                return (T)base[i];
            }
        }

        return null;
    }
}


