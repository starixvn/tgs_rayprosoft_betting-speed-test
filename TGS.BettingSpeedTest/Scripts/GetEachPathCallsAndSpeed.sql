SELECT
   "Path",
   COUNT("Path") AS "Calls",
   MIN("ExecutionTimeMs") AS "Min",
   MAX("ExecutionTimeMs") AS "Max",
   to_char(AVG("ExecutionTimeMs"), '99999D99') AS "Avg" 
FROM
   dbo."ApiCallLogs" 
WHERE
   "Prefix" = 'dopamine' 
   AND "ResponseCode" = 200 
   AND "RecordedOn" BETWEEN '2020-03-13 18:45:00.000+08:00' AND '2020-03-13 18:48:00.000+08:00' 
   AND "ExecutionTimeMs" > 0 
   AND 
   (
      "Path" LIKE '%bets/place%' 
      OR "Path" LIKE '%bets/settle%' 
   )
GROUP BY
   "Path" 
ORDER BY
   1 ;