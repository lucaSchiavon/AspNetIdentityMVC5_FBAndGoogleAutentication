# AspNetIdentityMVC5_FBAndGoogleAutentication
Come utilizzare asp net identity per accedere attraverso google e facebook e come reimpostare la password e ricevere conferma di identità dopo la registrazione confermando la email

La guida microsoft per attivare la google e fb autentication si trova qui:<br>
https://docs.microsoft.com/en-us/aspnet/mvc/overview/security/create-an-aspnet-mvc-5-app-with-facebook-and-google-oauth2-and-openid-sign-on<br>

ma molto utili sono questi video online:
per  impostrae il reset password:
https://www.youtube.com/watch?v=LcMaO170oec
per confermare l'email:
https://www.youtube.com/watch?v=Rq06iC45Bj0

per utilizzare l'smt di google per inviare messaggi di posta elettronica (per reimpostare la pwd o conferma email andare qui:
https://myaccount.google.com/lesssecureapps


NOTE informative:
logi FB funziona così:
se si decide di loggarsi con FB la prima volta dopo essere stati dirottati verso FB ed aver ricevuto le credenziali si deve fornire uno username con cui la nostra applicazione associerà la autenticazione di FB con un utente della security asnet che verrà associato a tale autenticazione mediante il suo providerkey (si veda tabella [AspNetUserLogins])
<br><br>inoltre la conferma email si usa per evitare due circostanze:<br>
che un boot non si registri per conto nostro, per evitare di avere inserito erroneamente un indirizzo email scorretto (in questo caso non potremo più cambiare la password perchè non riceveremo più l'email.
