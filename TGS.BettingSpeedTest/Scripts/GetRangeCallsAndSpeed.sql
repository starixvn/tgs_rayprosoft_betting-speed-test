WITH data AS 
(
   SELECT
      "Path",
      CASE
         WHEN
            "ExecutionTimeMs" BETWEEN 0 AND 199 
         THEN
            '0000 to 0199' 
         WHEN
            "ExecutionTimeMs" BETWEEN 200 AND 999 
         THEN
            '0200 to 0999' 
         WHEN
            "ExecutionTimeMs" BETWEEN 1000 AND 2999 
         THEN
            '1000 to 2999' 
         WHEN
            "ExecutionTimeMs" BETWEEN 3000 AND 6999 
         THEN
            '3000 to 6999' 
         ELSE
            '7000 and above' 
      END
      AS "Range" 
   FROM
      dbo."ApiCallLogs" 
   WHERE
      "Prefix" = 'dopamine' 
      AND "ResponseCode" = 200 
      AND "ExecutionTimeMs" > 0 
      AND "RecordedOn" BETWEEN '2020-03-13 18:45:00.000+08:00' AND '2020-03-13 18:48:00.000+08:00' 
      AND 
      (
         "Path" LIKE '%bets/place%' 
         OR "Path" LIKE '%bets/settle%' 
      )
)
SELECT
   t1."Path",
   t1."Range",
   COUNT(t1."Range") AS "Samples",
   (
      CAST(COUNT(t1."Range") AS FLOAT)*100 
   )
   / t2."SampleCount" AS "Percentage" 
FROM
   data t1 
   LEFT JOIN
      (
         SELECT
            "Path",
            COUNT("Range") AS "SampleCount" 
         FROM
            data 
         GROUP BY
            "Path" 
      )
      t2 
      ON t1."Path" = t2."Path" 
GROUP BY
   t1."Path",
   t1."Range",
   t2."SampleCount" 
ORDER BY
   "Path",
   "Range";