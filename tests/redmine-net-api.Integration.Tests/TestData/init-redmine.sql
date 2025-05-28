-- 1. Insert users
INSERT INTO users (id, login, hashed_password, salt, firstname, lastname, admin, status, type, created_on, updated_on)
VALUES (90, 'adminuser', '5cfe86e41de3a143be90ae5f7ced76841a0830bf', 'e71a2bcb922bede1becc396b326b93ff', 'Admin', 'User', true, 1, 'User', NOW(), NOW()),
       (91, 'normaluser', '3c4afd1d5042356c7fdd19e0527db108919624f9', '6030b2ed3c7eb797eb706a325bb227ad', 'Normal', 'User', false, 1, 'User', NOW(), NOW());

-- 2. Insert API keys
INSERT INTO tokens (user_id, action, value, created_on)
VALUES
    (90, 'api', '029a9d38-17e8-41ae-bc8c-fbf71e193c57', NOW()),
    (91, 'api', 'b94da108-c6d0-483a-9c21-2648fe54521d', NOW());

INSERT INTO settings (id, name, "value", updated_on)
values (99, 'rest_api_enabled', 1, now());

insert into enabled_modules (id, project_id, name)
values  (1, 1, 'issue_tracking'),
        (2, 1, 'time_tracking'),
        (3, 1, 'news'),
        (4, 1, 'documents'),
        (5, 1, 'files'),
        (6, 1, 'wiki'),
        (7, 1, 'repository'),
        (8, 1, 'boards'),
        (9, 1, 'calendar'),
        (10, 1, 'gantt');


insert into enumerations (id, name, position, is_default, type, active, project_id, parent_id, position_name)
values  (1, 'Low', 1, false, 'IssuePriority', true, null, null, 'lowest'),
        (2, 'Normal', 2, true, 'IssuePriority', true, null, null, 'default'),
        (3, 'High', 3, false, 'IssuePriority', true, null, null, 'high3'),
        (4, 'Urgent', 4, false, 'IssuePriority', true, null, null, 'high2'),
        (5, 'Immediate', 5, false, 'IssuePriority', true, null, null, 'highest'),
        (6, 'User documentation', 1, false, 'DocumentCategory', true, null, null, null),
        (7, 'Technical documentation', 2, false, 'DocumentCategory', true, null, null, null),
        (8, 'Design', 1, false, 'TimeEntryActivity', true, null, null, null),
        (9, 'Development', 2, false, 'TimeEntryActivity', true, null, null, null);

insert into issue_statuses (id, name, is_closed, position, default_done_ratio, description)
values  (1, 'New', false, 1, null, null),
        (2, 'In Progress', false, 2, null, null),
        (3, 'Resolved', false, 3, null, null),
        (4, 'Feedback', false, 4, null, null),
        (5, 'Closed', true, 5, null, null),
        (6, 'Rejected', true, 6, null, null);


insert into trackers (id, name, position, is_in_roadmap, fields_bits, default_status_id, description)
values  (1, 'Bug', 1, false, 0, 1, null),
        (2, 'Feature', 2, true, 0, 1, null),
        (3, 'Support', 3, false, 0, 1, null);

insert into projects (id, name, description, homepage, is_public, parent_id, created_on, updated_on, identifier, status, lft, rgt, inherit_members, default_version_id, default_assigned_to_id, default_issue_query_id)
values  (1, 'Project-Test', null, '', true, null, '2024-09-02 10:14:33.789394', '2024-09-02 10:14:33.789394', 'project-test', 1, 1, 2, false, null, null, null);

insert into public.wikis (id, project_id, start_page, status) values (1, 1, 'Wiki', 1);

insert into versions (id, project_id, name, description, effective_date, created_on, updated_on, wiki_page_title, status, sharing)
values  (1, 1, 'version1', '', null, '2025-04-28 17:56:49.245993', '2025-04-28 17:56:49.245993', '', 'open', 'none'),
        (2, 1, 'version2', '', null, '2025-04-28 17:57:05.138915', '2025-04-28 17:57:05.138915', '', 'open', 'descendants');

insert into issues (id, tracker_id, project_id, subject, description, due_date, category_id, status_id, assigned_to_id, priority_id, fixed_version_id, author_id, lock_version, created_on, updated_on, start_date, done_ratio, estimated_hours, parent_id, root_id, lft, rgt, is_private, closed_on)
values  (5, 1, 1, '#380', '', null, 1, 1, null, 2, 2, 90, 1, '2025-04-28 17:58:42.818731', '2025-04-28 17:58:42.818731', '2025-04-28', 0, null, null, 5, 1, 2, false, null),
        (6, 1, 1, 'issue with file', '', null, null, 1, null, 3, 2, 90, 1, '2025-04-28 18:00:07.296872', '2025-04-28 18:00:07.296872', '2025-04-28', 0, null, null, 6, 1, 2, false, null);

insert into watchers (id, watchable_type, watchable_id, user_id)
values  (8, 'Issue', 5, 90),
        (9, 'Issue', 5, 91);


