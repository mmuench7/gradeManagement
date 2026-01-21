USE grademanagement;

-- Prorektor -> Kategorien Mapping
DELETE FROM principal_job_category;

INSERT INTO principal_job_category (PrincipalId, JobCategoryId)
SELECT p.Id, jc.Id
FROM principal p
JOIN job_category jc
WHERE (p.Email='regula.tobler@gibz.ch' AND jc.Name IN ('Gesundheit','Soziales','Dienstleistung'))
   OR (p.Email='werner.odermatt@gibz.ch' AND jc.Name IN ('Automobil','Technik','Informatik'))
   OR (p.Email='peter.loetscher@gibz.ch' AND jc.Name IN ('Services'))
   OR (p.Email='patrick.zeiger@gibz.ch' AND jc.Name IN ('Planung','Infrastruktur','Innenausbau'))
   OR (p.Email='alex.kobel@gibz.ch' AND jc.Name IN ('BM','ABU','Sport'));

-- Dummy Grades (CourseId per Acronym, TeacherId per Email)
INSERT INTO grade (Value, ExamDate, Comment, StudentId, CourseId, TeacherId)
VALUES
(5.5,'2025-11-30','Semesterprüfung', 1, (SELECT Id FROM course WHERE Acronym='INF-01'), (SELECT Id FROM teacher WHERE Email='christian.lindauer@gibz.ch')),
(4.0,'2025-10-15','Test',            2, (SELECT Id FROM course WHERE Acronym='INF-02'), (SELECT Id FROM teacher WHERE Email='christian.lindauer@gibz.ch')),
(5.0,'2025-09-20','ABU Test',        3, (SELECT Id FROM course WHERE Acronym='ABU-01'), (SELECT Id FROM teacher WHERE Email='peter.gisler@gibz.ch')),
(5.5,'2025-12-05','Sport',           4, (SELECT Id FROM course WHERE Acronym='SPO-01'), (SELECT Id FROM teacher WHERE Email='peter.gisler@gibz.ch')),
(4.5,'2025-10-03','Service Test',    5, (SELECT Id FROM course WHERE Acronym='SER-01'), (SELECT Id FROM teacher WHERE Email='roger.mueller@gibz.ch')),
(5.0,'2025-11-18','Organisation',    6, (SELECT Id FROM course WHERE Acronym='SER-02'), (SELECT Id FROM teacher WHERE Email='roger.mueller@gibz.ch')),
(4.0,'2025-09-28','Planung',         7, (SELECT Id FROM course WHERE Acronym='PLA-01'), (SELECT Id FROM teacher WHERE Email='pascal.zumbuehl@gibz.ch')),
(5.5,'2025-10-22','Innenausbau',     8, (SELECT Id FROM course WHERE Acronym='INN-01'), (SELECT Id FROM teacher WHERE Email='andreja.torriani@gibz.ch'));
