--
-- PostgreSQL database dump
--

-- Dumped from database version 15.2
-- Dumped by pg_dump version 15.2

-- Started on 2023-04-13 15:43:43

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- TOC entry 219 (class 1259 OID 32841)
-- Name: game; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.game (
    gameid integer NOT NULL,
    userid integer NOT NULL,
    username character varying,
    date_game timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    moves integer DEFAULT 0,
    researched_states integer DEFAULT 0
);


ALTER TABLE public.game OWNER TO postgres;

--
-- TOC entry 218 (class 1259 OID 32840)
-- Name: game_gameid_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.game_gameid_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.game_gameid_seq OWNER TO postgres;

--
-- TOC entry 3268 (class 0 OID 0)
-- Dependencies: 218
-- Name: game_gameid_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.game_gameid_seq OWNED BY public.game.gameid;


--
-- TOC entry 215 (class 1259 OID 32780)
-- Name: information; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.information (
    informationid integer NOT NULL,
    title character varying,
    text_info text
);


ALTER TABLE public.information OWNER TO postgres;

--
-- TOC entry 214 (class 1259 OID 32779)
-- Name: information_informationid_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.information_informationid_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.information_informationid_seq OWNER TO postgres;

--
-- TOC entry 3269 (class 0 OID 0)
-- Dependencies: 214
-- Name: information_informationid_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.information_informationid_seq OWNED BY public.information.informationid;


--
-- TOC entry 217 (class 1259 OID 32827)
-- Name: users; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.users (
    userid integer NOT NULL,
    username character varying,
    login character varying NOT NULL,
    password_ character varying NOT NULL,
    date_registration timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    last_login_date date DEFAULT CURRENT_TIMESTAMP,
    total_games integer DEFAULT 0
);


ALTER TABLE public.users OWNER TO postgres;

--
-- TOC entry 216 (class 1259 OID 32826)
-- Name: users_userid_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.users_userid_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.users_userid_seq OWNER TO postgres;

--
-- TOC entry 3270 (class 0 OID 0)
-- Dependencies: 216
-- Name: users_userid_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.users_userid_seq OWNED BY public.users.userid;


--
-- TOC entry 3099 (class 2604 OID 32844)
-- Name: game gameid; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.game ALTER COLUMN gameid SET DEFAULT nextval('public.game_gameid_seq'::regclass);


--
-- TOC entry 3094 (class 2604 OID 32783)
-- Name: information informationid; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.information ALTER COLUMN informationid SET DEFAULT nextval('public.information_informationid_seq'::regclass);


--
-- TOC entry 3095 (class 2604 OID 32830)
-- Name: users userid; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.users ALTER COLUMN userid SET DEFAULT nextval('public.users_userid_seq'::regclass);


--
-- TOC entry 3262 (class 0 OID 32841)
-- Dependencies: 219
-- Data for Name: game; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO public.game (gameid, userid, username, date_game, moves, researched_states) VALUES (1, 1, 'Veronika', '2023-04-05 21:28:43.434297', 32, 50000);
INSERT INTO public.game (gameid, userid, username, date_game, moves, researched_states) VALUES (2, 1, 'Veronika', '2023-04-05 21:28:43.434297', 54, 65000);
INSERT INTO public.game (gameid, userid, username, date_game, moves, researched_states) VALUES (3, 3, 'Artem', '2023-04-05 21:28:43.434297', 78, 30200);
INSERT INTO public.game (gameid, userid, username, date_game, moves, researched_states) VALUES (7, 3, 'Artem', '2023-04-13 14:23:00.384891', 47, 286335);
INSERT INTO public.game (gameid, userid, username, date_game, moves, researched_states) VALUES (4, 1, 'Veronika', '2023-04-05 21:30:03.611999', 56, 200300);
INSERT INTO public.game (gameid, userid, username, date_game, moves, researched_states) VALUES (5, 1, 'Veronika', '2023-04-09 19:07:07.742079', 74, 345222);
INSERT INTO public.game (gameid, userid, username, date_game, moves, researched_states) VALUES (6, 3, 'Artem', '2023-04-13 14:22:04.248114', 45, 348888);


--
-- TOC entry 3258 (class 0 OID 32780)
-- Dependencies: 215
-- Data for Name: information; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO public.information (informationid, title, text_info) VALUES (1, 'Правила игры', 'Классическое игровое поле представляет собой матрицу 4х4 клеток, на котором по порядку (слева - направо и сверху - вниз) располагаются цифры от 1 до 15. 

Последняя клетка – пустая. Клетки перемешиваются определенным образом, и задача игрока состоит в том, чтобы восстановить их первоначальное правильное расположение.');
INSERT INTO public.information (informationid, title, text_info) VALUES (2, 'Про создателя пятнашек', 'Считается, что головоломку «Пятнашки» в 1870-х гг. создал шахматист и составитель шахматных задач Сэм Лойд (1841–1911). 

Она состоит из игрового поля размером N x M, в каждой клетке которого может быть всё что угодно: цифра, буква, изображение и т.д.');
INSERT INTO public.information (informationid, title, text_info) VALUES (3, 'Пятнашки в информатике', '«Пятнашки» разных размеров с 1960-х годов регулярно используются в исследованиях в области ИИ; 

в частности, на них испытываются и сравниваются алгоритмы поиска в пространстве состояний с разными эвристическими функциямии другими оптимизациями, влияющими на число посещённых в процессе поиска конфигураций головоломки (вершин графа).');
INSERT INTO public.information (informationid, title, text_info) VALUES (4, 'Новый век', 'Последние годы человек просто не может существовать без компьютера, смартфона и прочих гаджетов. 

Поэтому, разработчики активно этим пользуются и выпускают электронные версии популярных логических и настольных игр.

Пятнашки - не исключение. Сегодня без труда любой пользователь сможет загрузить игру на персональный компьютер или приложение на свой смартфон. 
Такие игры помогают скоротать время в транспорте по дороге на работу или в длинной очереди.');


--
-- TOC entry 3260 (class 0 OID 32827)
-- Dependencies: 217
-- Data for Name: users; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO public.users (userid, username, login, password_, date_registration, last_login_date, total_games) VALUES (2, 'Alina', 'Alina123', 'Alina123', '2023-04-05 21:28:43.434297', '2023-04-05', 0);
INSERT INTO public.users (userid, username, login, password_, date_registration, last_login_date, total_games) VALUES (4, 'Rinat', 'Rinat123', 'Rinat123', '2023-04-05 21:28:43.434297', '2023-04-05', 0);
INSERT INTO public.users (userid, username, login, password_, date_registration, last_login_date, total_games) VALUES (5, 'Lina', 'Lina123', 'Lina123', '2023-04-05 21:28:43.434297', '2023-04-05', 0);
INSERT INTO public.users (userid, username, login, password_, date_registration, last_login_date, total_games) VALUES (1, 'Veronika', 'Veronika123', 'Veronika123', '2023-04-05 21:28:43.434297', '2023-04-13', 4);
INSERT INTO public.users (userid, username, login, password_, date_registration, last_login_date, total_games) VALUES (3, 'Artem', 'Artem123', 'Artem123', '2023-04-05 21:28:43.434297', '2023-04-13', 3);


--
-- TOC entry 3271 (class 0 OID 0)
-- Dependencies: 218
-- Name: game_gameid_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.game_gameid_seq', 7, true);


--
-- TOC entry 3272 (class 0 OID 0)
-- Dependencies: 214
-- Name: information_informationid_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.information_informationid_seq', 4, true);


--
-- TOC entry 3273 (class 0 OID 0)
-- Dependencies: 216
-- Name: users_userid_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.users_userid_seq', 5, true);


--
-- TOC entry 3112 (class 2606 OID 32851)
-- Name: game game_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.game
    ADD CONSTRAINT game_pkey PRIMARY KEY (gameid, userid);


--
-- TOC entry 3104 (class 2606 OID 32787)
-- Name: information information_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.information
    ADD CONSTRAINT information_pkey PRIMARY KEY (informationid);


--
-- TOC entry 3106 (class 2606 OID 32789)
-- Name: information information_title_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.information
    ADD CONSTRAINT information_title_key UNIQUE (title);


--
-- TOC entry 3108 (class 2606 OID 32837)
-- Name: users users_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.users
    ADD CONSTRAINT users_pkey PRIMARY KEY (userid);


--
-- TOC entry 3110 (class 2606 OID 32839)
-- Name: users users_username_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.users
    ADD CONSTRAINT users_username_key UNIQUE (username);


--
-- TOC entry 3113 (class 2606 OID 32852)
-- Name: game game_userid_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.game
    ADD CONSTRAINT game_userid_fkey FOREIGN KEY (userid) REFERENCES public.users(userid);


--
-- TOC entry 3114 (class 2606 OID 32857)
-- Name: game game_username_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.game
    ADD CONSTRAINT game_username_fkey FOREIGN KEY (username) REFERENCES public.users(username);


-- Completed on 2023-04-13 15:43:44

--
-- PostgreSQL database dump complete
--

