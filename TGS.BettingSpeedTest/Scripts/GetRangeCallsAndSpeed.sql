WITH data AS 
(
   SELECT
      "path",
      CASE
         WHEN
            "execution_time_ms" BETWEEN 0 AND 199 
         THEN
            '0000 to 0199' 
         WHEN
            "execution_time_ms" BETWEEN 200 AND 999 
         THEN
            '0200 to 0999' 
         WHEN
            "execution_time_ms" BETWEEN 1000 AND 2999 
         THEN
            '1000 to 2999' 
         WHEN
            "execution_time_ms" BETWEEN 3000 AND 6999 
         THEN
            '3000 to 6999' 
         ELSE
            '7000 and above' 
      END
      AS "range" 
   FROM
      dbo."api_logs" 
   WHERE
      "prefix" = 'sbo' 
      AND "response_code" = 200 
      AND "execution_time_ms" > 0 
      AND "recorded_on" BETWEEN '2021-05-04 00:45:00.000+08:00' AND '2021-05-13 18:48:00.000+08:00' 
      AND 
      (
         "path" LIKE '%decuct%' 
         OR "path" LIKE '%settle%' 
      )
)

SELECT
   t1."path",
   t1."range",
   COUNT(t1."range") AS "samples",
   (
      CAST(COUNT(t1."range") AS FLOAT)*100 
   )
   / t2."sample_count" AS "percentage" 
FROM
   data t1 
   LEFT JOIN
      (
         SELECT
            "path",
            COUNT("range") AS "sample_count" 
         FROM
            data 
         GROUP BY
            "path" 
      )
      t2 
      ON t1."path" = t2."path" 
GROUP BY
   t1."path",
   t1."range",
   t2."sample_count" 
ORDER BY
   "path",
   "range";