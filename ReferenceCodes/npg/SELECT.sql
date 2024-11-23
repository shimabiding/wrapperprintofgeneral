SELECT
	a.mngcode,
	a.subnum,
	b.subj,
	c.C_amount,
	c.deliv,
	c.job_seq,
	d.cus_name
FROM (
	SELECT *
	FROM a
	WHERE orderid = 123417
	)a

JOIN (
	SELECT *
	FROM b
	)b
	ON a.bid = b.bid

JOIN (
	SELECT *
	FROM c
	)c
	ON a.bid = c.bid

JOIN (
	SELECT *
	FROM d
	)d
	ON b.cusid = d.cusid
	