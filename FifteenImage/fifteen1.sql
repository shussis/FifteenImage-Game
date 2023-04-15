--
-- PostgreSQL database dump
--

-- Dumped from database version 15.2
-- Dumped by pg_dump version 15.2

-- Started on 2023-04-15 17:48:58

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
-- TOC entry 219 (class 1259 OID 41031)
-- Name: game; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.game (
    gameid integer NOT NULL,
    userid integer NOT NULL,
    date_game timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    moves integer DEFAULT 0,
    researched_states integer DEFAULT 0
);


ALTER TABLE public.game OWNER TO postgres;

--
-- TOC entry 218 (class 1259 OID 41030)
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
-- TOC entry 3265 (class 0 OID 0)
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
-- TOC entry 3266 (class 0 OID 0)
-- Dependencies: 214
-- Name: information_informationid_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.information_informationid_seq OWNED BY public.information.informationid;


--
-- TOC entry 217 (class 1259 OID 41019)
-- Name: users; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.users (
    userid integer NOT NULL,
    username character varying NOT NULL,
    login character varying NOT NULL,
    password_ character varying NOT NULL,
    date_registration timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    last_login_date date DEFAULT CURRENT_TIMESTAMP,
    total_games integer DEFAULT 0
);


ALTER TABLE public.users OWNER TO postgres;

--
-- TOC entry 216 (class 1259 OID 41018)
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
-- TOC entry 3267 (class 0 OID 0)
-- Dependencies: 216
-- Name: users_userid_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.users_userid_seq OWNED BY public.users.userid;


--
-- TOC entry 3099 (class 2604 OID 41034)
-- Name: game gameid; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.game ALTER COLUMN gameid SET DEFAULT nextval('public.game_gameid_seq'::regclass);


--
-- TOC entry 3094 (class 2604 OID 32783)
-- Name: information informationid; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.information ALTER COLUMN informationid SET DEFAULT nextval('public.information_informationid_seq'::regclass);


--
-- TOC entry 3095 (class 2604 OID 41022)
-- Name: users userid; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.users ALTER COLUMN userid SET DEFAULT nextval('public.users_userid_seq'::regclass);


--
-- TOC entry 3259 (class 0 OID 41031)
-- Dependencies: 219
-- Data for Name: game; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.game (gameid, userid, date_game, moves, researched_states) FROM stdin;
1	1	2023-04-15 15:43:15.961152	32	50000
2	1	2023-04-15 15:43:15.961152	54	65000
3	3	2023-04-15 15:43:15.961152	78	30200
4	1	2023-04-15 16:37:05.287329	45	284999
5	1	2023-04-15 16:58:23.862968	47	234222
\.


--
-- TOC entry 3255 (class 0 OID 32780)
-- Dependencies: 215
-- Data for Name: information; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.information (informationid, title, text_info) FROM stdin;
1	Правила игры	Классическое игровое поле представляет собой матрицу 4х4 клеток, на котором по порядку (слева - направо и сверху - вниз) располагаются цифры от 1 до 15. \n\nПоследняя клетка – пустая. Клетки перемешиваются определенным образом, и задача игрока состоит в том, чтобы восстановить их первоначальное правильное расположение.
2	Про создателя пятнашек	Считается, что головоломку «Пятнашки» в 1870-х гг. создал шахматист и составитель шахматных задач Сэм Лойд (1841–1911). \n\nОна состоит из игрового поля размером N x M, в каждой клетке которого может быть всё что угодно: цифра, буква, изображение и т.д.
3	Пятнашки в информатике	«Пятнашки» разных размеров с 1960-х годов регулярно используются в исследованиях в области ИИ; \n\nв частности, на них испытываются и сравниваются алгоритмы поиска в пространстве состояний с разными эвристическими функциямии другими оптимизациями, влияющими на число посещённых в процессе поиска конфигураций головоломки (вершин графа).
4	Новый век	Последние годы человек просто не может существовать без компьютера, смартфона и прочих гаджетов. \n\nПоэтому, разработчики активно этим пользуются и выпускают электронные версии популярных логических и настольных игр.\n\nПятнашки - не исключение. Сегодня без труда любой пользователь сможет загрузить игру на персональный компьютер или приложение на свой смартфон. \nТакие игры помогают скоротать время в транспорте по дороге на работу или в длинной очереди.
\.


--
-- TOC entry 3257 (class 0 OID 41019)
-- Dependencies: 217
-- Data for Name: users; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.users (userid, username, login, password_, date_registration, last_login_date, total_games) FROM stdin;
2	Alina	Alina123	Alina123	2023-04-15 15:43:15.961152	2023-04-15	0
4	Rinat	Rinat123	Rinat123	2023-04-15 15:43:15.961152	2023-04-15	0
5	Lina	Lina123	Lina123	2023-04-15 15:43:15.961152	2023-04-15	0
3	Artem	Artem123	Artem123	2023-04-15 15:43:15.961152	2023-04-15	1
1	Veronika	Veronika123	Veronika123	2023-04-15 15:43:15.961152	2023-04-15	4
6	Amina	Amina123	Amina123	2023-04-15 17:06:37.561084	2023-04-15	0
\.


--
-- TOC entry 3268 (class 0 OID 0)
-- Dependencies: 218
-- Name: game_gameid_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.game_gameid_seq', 5, true);


--
-- TOC entry 3269 (class 0 OID 0)
-- Dependencies: 214
-- Name: information_informationid_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.information_informationid_seq', 4, true);


--
-- TOC entry 3270 (class 0 OID 0)
-- Dependencies: 216
-- Name: users_userid_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.users_userid_seq', 6, true);


--
-- TOC entry 3110 (class 2606 OID 41039)
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
-- TOC entry 3108 (class 2606 OID 41029)
-- Name: users users_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.users
    ADD CONSTRAINT users_pkey PRIMARY KEY (userid);


--
-- TOC entry 3111 (class 2606 OID 41040)
-- Name: game game_userid_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.game
    ADD CONSTRAINT game_userid_fkey FOREIGN KEY (userid) REFERENCES public.users(userid);


-- Completed on 2023-04-15 17:48:58

--
-- PostgreSQL database dump complete
--

