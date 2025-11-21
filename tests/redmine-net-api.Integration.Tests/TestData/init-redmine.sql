BEGIN;

INSERT INTO public."settings" (name, value, updated_on) 
VALUES
    ('autofetch_changesets',            '1',                        NOW()),
    ('rest_api_enabled',                '1',                        NOW()),
    ('jsonp_enabled',                   '0',                        NOW()),
    ('issues_export_limit',             '1000',                     NOW()),
    ('user_format',                     'firstname_lastname',       NOW()),
    ('welcome_text',                    'Integration Test Instance',NOW()),
    ('search_results_per_page',         '50',                       NOW()),
    ('search_on_rendering_enabled',     '0',                        NOW()),
    ('sequential_project_identifiers',  '1',                        NOW()),
    ('sys_api_enabled',                 '0',                        NOW()),
    ('background_jobs_queue_adapter',   'delayed_job',              NOW()),
    ('background_jobs_polling_interval','5',                        NOW()),
    ('mail_delivery_method',            'smtp',                     NOW()),
    ('mail_handler_api_enabled',        '0',                        NOW()),
    ('attachment_max_size',             '5120',                     NOW()),  -- 5 MB
    ('default_language',                'en',                       NOW()),
    ('default_notification_option',     'only_assigned',            NOW()),
    ('default_search_scope',            'all',                      NOW()),
    ('default_projects_public',         '1',                        NOW()),
    ('default_projects_modules',        $$---
- issue_tracking
- time_tracking
- news
- documents
- files
- wiki
$$, now());
        
WITH maybe_project AS (
    SELECT id
    FROM public."projects"
    WHERE identifier = 'it-redmine'
)
INSERT INTO public."projects" (
    name,
    identifier,
    description,
    is_public,
    lft,
    rgt,
    inherit_members,
    status,
    created_on,
    updated_on
)
SELECT
    'Integration Tests',
    'it-redmine',
    'Project used for automated integration tests.',
    FALSE,
    1,
    2,
    FALSE,
    1,              -- active
    NOW(),
    NOW()
WHERE NOT EXISTS (SELECT 1 FROM maybe_project);

INSERT INTO public."enabled_modules" (project_id, name)
SELECT p.id, m.name
FROM public."projects" p
CROSS JOIN (VALUES
     ('issue_tracking'),
     ('time_tracking'),
     ('news'),
     ('documents'),
     ('files'),
     ('wiki'),
     ('repository'),
     ('boards'),
     ('calendar'),
     ('gantt')
) AS m(name)
WHERE p.identifier = 'it-redmine';

INSERT INTO public."projects_trackers" (project_id, tracker_id)
SELECT p.id, t.id
FROM public."projects" p
JOIN public."trackers" t ON 1 = 1
WHERE p.identifier = 'it-redmine';

INSERT INTO public."issue_categories" (project_id, name, assigned_to_id)
SELECT p.id, c.name, NULL
FROM public."projects" p
JOIN (VALUES ('Bug'), ('Feature')) AS c(name) ON 1 = 1
WHERE p.identifier = 'it-redmine';

INSERT INTO public."versions" (project_id, name, status, sharing, created_on, updated_on)
SELECT p.id, v.name, v.status, v.sharing, NOW(), NOW()
FROM public."projects" p
JOIN (VALUES
       ('1.0', 'open',   'none'),
       ('2.0', 'locked', 'none')
) AS v(name, status, sharing) ON 1 = 1
WHERE p.identifier = 'it-redmine';

INSERT INTO public."repositories" (
    project_id,
    url,
    root_url,
    type,
    identifier,
    is_default,
    created_on
)
SELECT
    p.id,
    '/var/redmine/git_repositories/it-redmine.git',
    '/var/redmine/git_repositories/it-redmine.git',
    'Repository::Git',
    'git-main',
    TRUE,
    NOW()
FROM public."projects" p
WHERE p.identifier = 'it-redmine';

UPDATE public."users"
SET "hashed_password" = '5cfe86e41de3a143be90ae5f7ced76841a0830bf',
    "salt" = 'e71a2bcb922bede1becc396b326b93ff',
    "must_change_passwd" = false,
    admin = true
WHERE id = 1;

INSERT INTO public."members" (project_id, user_id, created_on)
SELECT p.id, 1, NOW()
FROM public."projects" p
WHERE p.identifier = 'it-redmine'
  AND NOT EXISTS (
    SELECT 1 FROM public."members" m
    WHERE m.project_id = p.id AND m.user_id = 1
);

INSERT INTO public."member_roles" (member_id, role_id)
SELECT m.id, r.id
FROM public."members" m
JOIN public."projects" p ON p.id = m.project_id
JOIN public."roles" r ON r.name = 'Manager' AND r.builtin = 0
WHERE p.identifier = 'it-redmine'
  AND m.user_id = 1
  AND NOT EXISTS (
    SELECT 1 FROM public."member_roles" mr
    WHERE mr.member_id = m.id AND mr.role_id = r.id
);

INSERT INTO public."tokens" (user_id, action, value, created_on)
VALUES (1, 'api', '61d6fa45ca2c570372b08b8c54b921e5fc39335a', NOW());

INSERT INTO public."issues" (tracker_id, project_id, subject, description, due_date, category_id, status_id, assigned_to_id, priority_id, fixed_version_id, author_id, lock_version, created_on, updated_on, start_date, done_ratio, estimated_hours, parent_id, root_id, lft, rgt, is_private, closed_on)
VALUES  ( 1, 1, '#380', '', null, 1, 1, null, 2, 2, 1, 1, '2025-04-28 17:58:42.818731', '2025-04-28 17:58:42.818731', '2025-04-28', 0, null, null, 5, 1, 2, false, null),
        ( 1, 1, 'issue with file', '', null, null, 1, null, 3, 2, 1, 1, '2025-04-28 18:00:07.296872', '2025-04-28 18:00:07.296872', '2025-04-28', 0, null, null, 6, 1, 2, false, null);

INSERT INTO public."watchers" (watchable_type, watchable_id, user_id)
VALUES  ( 'Issue', 5, 1),
        ( 'Issue', 5, 2);

INSERT INTO public."custom_fields" ("type", "name", field_format, min_length, max_length, is_required, is_for_all, is_filter, visible, multiple, description)
VALUES('IssueCustomField', 'IssueCustomField2', 'string', 2, 7,false, false, false,  true, false, 'ICF2 description'),
      ('IssueCustomField', 'IssueCustomField3', 'string', 2, 7,false, true, true,  true, true, 'ICF3 description');

INSERT INTO wikis (project_id, start_page, status)
SELECT id, 'Home', 1
FROM projects
WHERE identifier = 'it-redmine'
ON CONFLICT DO NOTHING;

INSERT INTO wiki_pages (wiki_id, title, created_on)
SELECT w.id, 'Home', NOW()
FROM wikis w
         JOIN projects p ON p.id = w.project_id
WHERE p.identifier = 'it-redmine'
  AND NOT EXISTS (
    SELECT 1 FROM wiki_pages wp WHERE wp.wiki_id = w.id AND wp.title = 'Home'
);

INSERT INTO wiki_contents (page_id, text, author_id, updated_on, version)
SELECT wp.id, 'Initial wiki page', 1, NOW(), 1
FROM wiki_pages wp
         JOIN wikis w ON w.id = wp.wiki_id
         JOIN projects p ON p.id = w.project_id
WHERE p.identifier = 'it-redmine'
  AND wp.title = 'Home'
  AND NOT EXISTS (
    SELECT 1 FROM wiki_contents wc WHERE wc.page_id = wp.id
);

COMMIT;