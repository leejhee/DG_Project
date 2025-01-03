using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;



namespace Client
{
    /// <summary>
    /// Singleton ��ü�� ���� ���̽� Ŭ���� 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Singleton<T> where T : class
    {
        /// <summary>
        /// Singleton ��ü ȣ��
        /// </summary>
        public static T Instance
        {
            get
            {
                return SingletonAllocator._Instance;
            }
        }

        public virtual void Init() { }
        #region ������
        /// <summary>
        /// Singleton ������
        /// </summary>
        protected Singleton() { }
        #endregion ������

        #region Singleton�� �������� Ŭ����
        /// <summary>
        /// Singleton�� �������� Ŭ����
        /// </summary>
        internal static class SingletonAllocator
        {
            internal static T _Instance = null; // Singleton ��ü

            /// <summary>
            /// SingletonAllocator ������ 
            /// </summary>
            static SingletonAllocator()
            {
                CreateInstance(typeof(T));
                Initialize(typeof(T));
            }

            /// <summary>
            /// _Instance ���� 
            /// </summary>
            /// <param name="type"> ���� Ÿ�� </param>
            static void CreateInstance(Type type)
            {
                ConstructorInfo constructorNonPublic = type.GetConstructor(
                    BindingFlags.Instance | BindingFlags.NonPublic, null, new Type[0], new ParameterModifier[0]);

                ConstructorInfo[] constructorPublic = type.GetConstructors(
                    BindingFlags.Instance | BindingFlags.Public);

                if (constructorPublic != null && constructorPublic.Length > 0)
                {
                    Debug.LogError($"{type.FullName}�� �����ڸ� Ȯ�����ּ���. Singleton �� Public �����ڸ� ������� �ʽ��ϴ�. ");
                    return;
                }

                // �����ڿ��� �ν��Ͻ��� �����մϴ�.
                if (constructorNonPublic != null)
                {
                    _Instance = (T)constructorNonPublic.Invoke(null);
                }
                else
                {
                    Debug.LogError("Singleton ��ü�� ������ �� �����ϴ�. ����� �����ڰ� �ʿ��մϴ�.");
                }
            }

            // Singleton ������Ʈ ���� ����
            static void Initialize(Type type)
            {

            }

        }
        #endregion Singleton�� �������� Ŭ����

    }
}