using System;
using UnityEngine;

public class AnalogClockKoreaTime : MonoBehaviour
{
    public Transform HourHand;
    public Transform MinuteHand;

    public Vector3 RotationAxis = new Vector3(0f, 0f, 1f);
    public float RotationDirection = -1f;

    public float ResyncIntervalMinutes = 10f;

    private TimeZoneInfo _kstTz;
    private double _baseSecondsOfDay;
    private float  _baseRealtime;
    private float  _nextResyncRt = float.MaxValue;

    void Awake()
    {
        _kstTz = ResolveKstTimeZone();
        SyncWithSystemTime();
    }

    void OnEnable()
    {
        if (ResyncIntervalMinutes <= 0f)
            SyncWithSystemTime();
    }

    void Update()
    {
        if (ResyncIntervalMinutes > 0f && Time.realtimeSinceStartup >= _nextResyncRt)
            SyncWithSystemTime();

        UpdateHands();
    }

    private static TimeZoneInfo ResolveKstTimeZone()
    {
        try
        {
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
            return TimeZoneInfo.FindSystemTimeZoneById("Korea Standard Time");
#else
            return TimeZoneInfo.FindSystemTimeZoneById("Asia/Seoul");
#endif
        }
        catch { /* fall through */ }

        try
        {
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
            return TimeZoneInfo.FindSystemTimeZoneById("Asia/Seoul");
#else
            return TimeZoneInfo.FindSystemTimeZoneById("Korea Standard Time");
#endif
        }
        catch { /* fall through */ }

        Debug.LogWarning("[AnalogClockKoreaTime] KST 타임존 ID를 찾지 못해 UTC+9 커스텀 타임존으로 폴백");
        return TimeZoneInfo.CreateCustomTimeZone(
            "KST",
            TimeSpan.FromHours(9),
            "Korea Standard Time",
            "Korea Standard Time"
        );
    }

    private void SyncWithSystemTime()
    {
        var utcNow   = DateTime.UtcNow;
        var koreaNow = TimeZoneInfo.ConvertTimeFromUtc(utcNow, _kstTz);

        TimeSpan tod = koreaNow.TimeOfDay;
        _baseSecondsOfDay = tod.TotalSeconds;
        _baseRealtime     = Time.realtimeSinceStartup;

        if (ResyncIntervalMinutes > 0f)
            _nextResyncRt = _baseRealtime + ResyncIntervalMinutes * 60f;

        UpdateHands();
    }

    private void UpdateHands()
    {
        if (HourHand == null && MinuteHand == null) return;

        float elapsed = Time.realtimeSinceStartup - _baseRealtime;
        double secOfDay = _baseSecondsOfDay + elapsed;

        const double secondsPerDay = 24 * 60 * 60;
        secOfDay %= secondsPerDay;
        if (secOfDay < 0) secOfDay += secondsPerDay;

        double hours   = secOfDay / 3600.0;     // 0~24
        double minutes = (secOfDay / 60.0) % 60;// 0~60, 소수 포함
        double hour12  = hours % 12.0;

        float hourAngle   = (float)(hour12  * 30.0) * RotationDirection;
        float minuteAngle = (float)(minutes * 6.0 ) * RotationDirection;

        if (HourHand   != null) HourHand.localRotation   = Quaternion.AngleAxis(hourAngle,   RotationAxis);
        if (MinuteHand != null) MinuteHand.localRotation = Quaternion.AngleAxis(minuteAngle, RotationAxis);
    }
}
