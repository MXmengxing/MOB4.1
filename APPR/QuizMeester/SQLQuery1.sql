-- Vraag 2
INSERT INTO questions (category_id, question_text, per_question_seconds)
VALUES (1, N'Wat is de hoofdstad van Duitsland?', 15);
DECLARE @qid INT = SCOPE_IDENTITY();
INSERT INTO answers (question_id, answer_text, is_correct)
VALUES 
(@qid, N'Berlijn', 1),
(@qid, N'München', 0),
(@qid, N'Hamburg', 0),
(@qid, N'Keulen', 0);

-- Vraag 3
INSERT INTO questions (category_id, question_text, per_question_seconds)
VALUES (2, N'Hoeveel poten heeft een spin?', 15);
SET @qid = SCOPE_IDENTITY();
INSERT INTO answers (question_id, answer_text, is_correct)
VALUES 
(@qid, N'8', 1),
(@qid, N'6', 0),
(@qid, N'10', 0),
(@qid, N'12', 0);

-- Vraag 4
INSERT INTO questions (category_id, question_text, per_question_seconds)
VALUES (3, N'Welke planeet is het dichtst bij de zon?', 15);
SET @qid = SCOPE_IDENTITY();
INSERT INTO answers (question_id, answer_text, is_correct)
VALUES 
(@qid, N'Mercurius', 1),
(@qid, N'Venus', 0),
(@qid, N'Mars', 0),
(@qid, N'Aarde', 0);

-- Vraag 5
INSERT INTO questions (category_id, question_text, per_question_seconds)
VALUES (1, N'Welke sport gebruikt een shuttle?', 15);
SET @qid = SCOPE_IDENTITY();
INSERT INTO answers (question_id, answer_text, is_correct)
VALUES 
(@qid, N'Badminton', 1),
(@qid, N'Tennis', 0),
(@qid, N'Voetbal', 0),
(@qid, N'Basketbal', 0);

-- Vraag 6
INSERT INTO questions (category_id, question_text, per_question_seconds)
VALUES (2, N'Wie schreef “De Nachtwacht”?', 15);
SET @qid = SCOPE_IDENTITY();
INSERT INTO answers (question_id, answer_text, is_correct)
VALUES 
(@qid, N'Rembrandt', 1),
(@qid, N'Van Gogh', 0),
(@qid, N'Mondriaan', 0),
(@qid, N'Rubens', 0);

-- Vraag 7
INSERT INTO questions (category_id, question_text, per_question_seconds)
VALUES (3, N'Hoeveel dagen heeft een schrikkeljaar?', 15);
SET @qid = SCOPE_IDENTITY();
INSERT INTO answers (question_id, answer_text, is_correct)
VALUES 
(@qid, N'366', 1),
(@qid, N'365', 0),
(@qid, N'364', 0),
(@qid, N'367', 0);

-- Vraag 8
INSERT INTO questions (category_id, question_text, per_question_seconds)
VALUES (1, N'In welk land vind je de stad Kyoto?', 15);
SET @qid = SCOPE_IDENTITY();
INSERT INTO answers (question_id, answer_text, is_correct)
VALUES 
(@qid, N'Japan', 1),
(@qid, N'China', 0),
(@qid, N'Korea', 0),
(@qid, N'Thailand', 0);

-- Vraag 9
INSERT INTO questions (category_id, question_text, per_question_seconds)
VALUES (2, N'Welke kleur krijg je als je blauw en geel mengt?', 15);
SET @qid = SCOPE_IDENTITY();
INSERT INTO answers (question_id, answer_text, is_correct)
VALUES 
(@qid, N'Groen', 1),
(@qid, N'Oranje', 0),
(@qid, N'Paars', 0),
(@qid, N'Bruin', 0);

-- Vraag 10
INSERT INTO questions (category_id, question_text, per_question_seconds)
VALUES (3, N'Wat is de grootste oceaan op aarde?', 15);
SET @qid = SCOPE_IDENTITY();
INSERT INTO answers (question_id, answer_text, is_correct)
VALUES 
(@qid, N'Stille Oceaan', 1),
(@qid, N'Atlantische Oceaan', 0),
(@qid, N'Indische Oceaan', 0),
(@qid, N'Arctische Oceaan', 0);

-- Vraag 11
INSERT INTO questions (category_id, question_text, per_question_seconds)
VALUES (2, N'Hoeveel tanden heeft een volwassen mens normaal?', 15);
SET @qid = SCOPE_IDENTITY();
INSERT INTO answers (question_id, answer_text, is_correct)
VALUES 
(@qid, N'32', 1),
(@qid, N'30', 0),
(@qid, N'28', 0),
(@qid, N'36', 0);
