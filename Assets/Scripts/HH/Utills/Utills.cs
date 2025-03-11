using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Utills : MonoBehaviour
{
    
}
public class Singleton<T> : MonoBehaviour where T : MonoBehaviour   // 싱글톤을 적용시킬 클래스에
{
    private static T instance;
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType(typeof(T)) as T;
                if (instance == null)
                    Debug.LogError("Not Activated : " + typeof(T));
            }
            return instance;
        }
    }
}
public class Data<T>//옵저버 패턴, Value가 변할때마다 메소드 호출
{
    private T v;//이게 변할때마다 메소드 호출
    public T Value
    {
        get { return v; }//밸류를 받아옴
        set//밸류가 변할때마다
        {
            v = value;
            onChange?.Invoke(value);//데이터에 구독된 함수를 호출
        }
    }
    public Action<T> onChange;//onChange로 데이터 안에 메소드 추가 및 제거(구독)
}
static class YieldCache 
{
    class FloatComparer : IEqualityComparer<float>
    {
        bool IEqualityComparer<float>.Equals(float x, float y)//부동 소수점 연산차이를 보정하기위한 과정. ex) 0.1f와 0.10000f가 다르게 취급되는걸 방지.
        {
            return x == y;
        }
        int IEqualityComparer<float>.GetHashCode(float obj)
        {
            return obj.GetHashCode();//해시값 반환으로 키를 효율적으로 검색하기위함
        }
    }

    public static readonly WaitForEndOfFrame WaitForEndOfFrame = new WaitForEndOfFrame();
    public static readonly WaitForFixedUpdate WaitForFixedUpdate = new WaitForFixedUpdate();

    //요약 : 동일한 대기시간을 사용할경우 객체를 새로 만들지 않고 이미 만들어져있는 객체에서 호출, 요청된 시간이 없다면 새로운 객체 생성 후 반환.
    private static readonly Dictionary<float, WaitForSeconds> _timeInterval = new Dictionary<float, WaitForSeconds>(new FloatComparer());//대기시간을 키를 둔 waitforseconds 객체저장(Dictionary사용)
    private static readonly Dictionary<float, WaitForSecondsRealtime> _timeIntervalReal = new Dictionary<float, WaitForSecondsRealtime>(new FloatComparer());

    public static WaitForSeconds WaitForSeconds(float seconds)//파라미터 seconds 값을 키로 설정 후 이미 캐싱된 객체 탐색
    {
        WaitForSeconds wfs;
        if (!_timeInterval.TryGetValue(seconds, out wfs))//존재하지않으면
            _timeInterval.Add(seconds, wfs = new WaitForSeconds(seconds));//딕셔너리에 저장
        return wfs;//반환
    }

    public static WaitForSecondsRealtime WaitForSecondsRealTime(float seconds)
    {
        WaitForSecondsRealtime wfsReal;
        if (!_timeIntervalReal.TryGetValue(seconds, out wfsReal))
            _timeIntervalReal.Add(seconds, wfsReal = new WaitForSecondsRealtime(seconds));
        return wfsReal;
    }
}
