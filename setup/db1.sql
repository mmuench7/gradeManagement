DROP SCHEMA IF EXISTS `grademanagement`;
CREATE SCHEMA `grademanagement`;
USE `grademanagement`;

-- Drop Reihenfolge: erst Link-Tabellen, dann abhängige, dann Basis
DROP TABLE IF EXISTS `teacher_course`;
DROP TABLE IF EXISTS `teacher_job_category`;
DROP TABLE IF EXISTS `principal_job_category`;
DROP TABLE IF EXISTS `grade_change_request`;
DROP TABLE IF EXISTS `grade`;
DROP TABLE IF EXISTS `student_class`;
DROP TABLE IF EXISTS `student`;
DROP TABLE IF EXISTS `school_class`;
DROP TABLE IF EXISTS `course`;
DROP TABLE IF EXISTS `teacher`;
DROP TABLE IF EXISTS `principal`;
DROP TABLE IF EXISTS `job_category`;

CREATE TABLE `job_category` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Name` varchar(100) NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `uq_job_category_name` (`Name`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE `principal` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Email` varchar(150) NOT NULL,
  `FirstName` varchar(100) NOT NULL,
  `LastName` varchar(100) NOT NULL,
  `PasswordHash` varchar(255) NOT NULL,
  `JobCategoryId` int NOT NULL,
  `PasswordSalt` varchar(255) NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `uq_principal_email` (`Email`),
  KEY `idx_principal_job_category_id` (`JobCategoryId`),
  CONSTRAINT `fk_principal_job_category`
    FOREIGN KEY (`JobCategoryId`) REFERENCES `job_category` (`Id`)
    ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE `teacher` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Email` varchar(150) NOT NULL,
  `FirstName` varchar(80) NOT NULL,
  `LastName` varchar(80) NOT NULL,
  `PasswordHash` varchar(255) NOT NULL,
  `PasswordSalt` varchar(255) NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `uq_teacher_email` (`Email`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE `course` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Name` varchar(150) NOT NULL,
  `Acronym` varchar(20) NOT NULL,
  `JobCategoryId` int NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `uq_course_acronym` (`Acronym`),
  KEY `fk_course_jc_idx` (`JobCategoryId`),
  CONSTRAINT `fk_course_jc`
    FOREIGN KEY (`JobCategoryId`) REFERENCES `job_category` (`Id`)
    ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE `school_class` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Name` varchar(20) NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `uq_school_class_name` (`Name`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE `student` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `FirstName` varchar(80) NOT NULL,
  `LastName` varchar(80) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE `student_class` (
  `StudentId` int NOT NULL,
  `ClassId` int NOT NULL,
  PRIMARY KEY (`StudentId`,`ClassId`),
  KEY `fk_sc_class` (`ClassId`),
  CONSTRAINT `fk_sc_class`
    FOREIGN KEY (`ClassId`) REFERENCES `school_class` (`Id`)
    ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `fk_sc_student`
    FOREIGN KEY (`StudentId`) REFERENCES `student` (`Id`)
    ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE `teacher_course` (
  `TeacherId` int NOT NULL,
  `CourseId` int NOT NULL,
  PRIMARY KEY (`TeacherId`,`CourseId`),
  KEY `fk_tc_course` (`CourseId`),
  CONSTRAINT `fk_tc_course`
    FOREIGN KEY (`CourseId`) REFERENCES `course` (`Id`)
    ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `fk_tc_teacher`
    FOREIGN KEY (`TeacherId`) REFERENCES `teacher` (`Id`)
    ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE `teacher_job_category` (
  `TeacherId` int NOT NULL,
  `JobCategoryId` int NOT NULL,
  PRIMARY KEY (`TeacherId`,`JobCategoryId`),
  KEY `fk_tjc_job_category` (`JobCategoryId`),
  CONSTRAINT `fk_tjc_job_category`
    FOREIGN KEY (`JobCategoryId`) REFERENCES `job_category` (`Id`)
    ON DELETE RESTRICT,
  CONSTRAINT `fk_tjc_teacher`
    FOREIGN KEY (`TeacherId`) REFERENCES `teacher` (`Id`)
    ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE `principal_job_category` (
  `PrincipalId` int NOT NULL,
  `JobCategoryId` int NOT NULL,
  PRIMARY KEY (`PrincipalId`, `JobCategoryId`),
  KEY `fk_pjc_job_category` (`JobCategoryId`),
  CONSTRAINT `fk_pjc_principal`
    FOREIGN KEY (`PrincipalId`) REFERENCES `principal` (`Id`)
    ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `fk_pjc_job_category`
    FOREIGN KEY (`JobCategoryId`) REFERENCES `job_category` (`Id`)
    ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE `grade` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Value` decimal(3,1) NOT NULL,
  `ExamDate` date NOT NULL,
  `Comment` varchar(255) DEFAULT NULL,
  `StudentId` int NOT NULL,
  `CourseId` int NOT NULL,
  `TeacherId` int NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `fk_grade_course` (`CourseId`),
  KEY `fk_grade_student` (`StudentId`),
  KEY `fk_grade_teacher` (`TeacherId`),
  CONSTRAINT `fk_grade_course`
    FOREIGN KEY (`CourseId`) REFERENCES `course` (`Id`)
    ON DELETE RESTRICT ON UPDATE CASCADE,
  CONSTRAINT `fk_grade_student`
    FOREIGN KEY (`StudentId`) REFERENCES `student` (`Id`)
    ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `fk_grade_teacher`
    FOREIGN KEY (`TeacherId`) REFERENCES `teacher` (`Id`)
    ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE `grade_change_request` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `GradeId` int NOT NULL,
  `TeacherId` int NOT NULL,
  `PrincipalId` int NOT NULL,
  `OriginalGradeValue` decimal(4,2) NOT NULL,
  `RequestedGradeValue` decimal(4,2) NOT NULL,
  `Reason` varchar(500) DEFAULT NULL,
  `PrincipalComment` varchar(500) DEFAULT NULL,
  `Status` int NOT NULL,
  `CreatedAt` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `ReviewedAt` datetime DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `fk_gcr_grade` (`GradeId`),
  KEY `fk_gcr_principal` (`PrincipalId`),
  KEY `fk_gcr_teacher` (`TeacherId`),
  CONSTRAINT `fk_gcr_grade`
    FOREIGN KEY (`GradeId`) REFERENCES `grade` (`Id`)
    ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `fk_gcr_principal`
    FOREIGN KEY (`PrincipalId`) REFERENCES `principal` (`Id`)
    ON DELETE RESTRICT ON UPDATE CASCADE,
  CONSTRAINT `fk_gcr_teacher`
    FOREIGN KEY (`TeacherId`) REFERENCES `teacher` (`Id`)
    ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- 13 JobCategories (wie Prorektorate)
INSERT INTO job_category (Name) VALUES
('Gesundheit'),
('Soziales'),
('Dienstleistung'),
('Automobil'),
('Technik'),
('Informatik'),
('Services'),
('Planung'),
('Infrastruktur'),
('Innenausbau'),
('BM'),
('ABU'),
('Sport');

-- Courses (JobCategoryId sauber via Name)
-- Informatik
INSERT INTO course (Name, Acronym, JobCategoryId) VALUES
('Programmieren', 'INF-01', (SELECT Id FROM job_category WHERE Name='Informatik')),
('Datenbanken',   'INF-02', (SELECT Id FROM job_category WHERE Name='Informatik'));

-- Technik
INSERT INTO course (Name, Acronym, JobCategoryId) VALUES
('Elektrotechnik', 'TEC-01', (SELECT Id FROM job_category WHERE Name='Technik')),
('Mechanik',       'TEC-02', (SELECT Id FROM job_category WHERE Name='Technik'));

-- ABU / Sport
INSERT INTO course (Name, Acronym, JobCategoryId) VALUES
('ABU Sprache',  'ABU-01', (SELECT Id FROM job_category WHERE Name='ABU')),
('Sport Theorie','SPO-01', (SELECT Id FROM job_category WHERE Name='Sport'));

-- Gesundheit/Soziales/Dienstleistung
INSERT INTO course (Name, Acronym, JobCategoryId) VALUES
('Gesundheit Grundlagen', 'GES-01', (SELECT Id FROM job_category WHERE Name='Gesundheit')),
('Soziales Kommunikation', 'SOZ-01', (SELECT Id FROM job_category WHERE Name='Soziales')),
('Dienstleistung Basics',  'DIE-01', (SELECT Id FROM job_category WHERE Name='Dienstleistung'));

-- Planung/Infrastruktur/Innenausbau
INSERT INTO course (Name, Acronym, JobCategoryId) VALUES
('Planung Basics', 'PLA-01', (SELECT Id FROM job_category WHERE Name='Planung')),
('Infrastruktur',  'INFRA-01', (SELECT Id FROM job_category WHERE Name='Infrastruktur')),
('Innenausbau',    'INN-01', (SELECT Id FROM job_category WHERE Name='Innenausbau'));

-- Services
INSERT INTO course (Name, Acronym, JobCategoryId) VALUES
('Kundenservice', 'SER-01', (SELECT Id FROM job_category WHERE Name='Services')),
('Organisation',  'SER-02', (SELECT Id FROM job_category WHERE Name='Services'));

-- Klassen & Schüler
INSERT INTO school_class (Name) VALUES ('IT23a'), ('IT23b'), ('GS24a');

INSERT INTO student (FirstName, LastName) VALUES
('Lena','Meier'),
('Noah','Keller'),
('Mia','Fischer'),
('Luca','Weber'),
('Sara','Schmid'),
('Tim','Huber'),
('Nina','Müller'),
('Jonas','Baumann'),
('Lea','Koch'),
('Elias','Brunner');

INSERT INTO student_class (StudentId, ClassId) VALUES
(1,1),(2,1),(3,1),
(4,2),(5,2),(6,2),
(7,3),(8,3),(9,3),(10,3);