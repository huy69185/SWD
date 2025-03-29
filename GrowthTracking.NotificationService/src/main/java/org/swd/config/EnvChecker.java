// File: src/main/java/org/swd/EnvChecker.java
package org.swd.config;

import org.springframework.boot.CommandLineRunner;
import org.springframework.core.env.Environment;
import org.springframework.stereotype.Component;

@Component
public class EnvChecker implements CommandLineRunner {

    private final Environment env;

    public EnvChecker(Environment env) {
        this.env = env;
    }

    @Override
    public void run(String... args) throws Exception {
        System.out.println("MAIL_HOST: " + env.getProperty("MAIL_HOST"));
        System.out.println("MAIL_PORT: " + env.getProperty("MAIL_PORT"));
        System.out.println("MAIL_USERNAME: " + env.getProperty("MAIL_USERNAME"));
        System.out.println("MAIL_PASSWORD: " + env.getProperty("MAIL_PASSWORD"));
        System.out.println("TWILIO_ACCOUNT_SID: " + env.getProperty("TWILIO_ACCOUNT_SID"));
        System.out.println("TWILIO_AUTH_TOKEN: " + env.getProperty("TWILIO_AUTH_TOKEN"));
        System.out.println("TWILIO_PHONE_NUMBER: " + env.getProperty("TWILIO_PHONE_NUMBER"));
        System.out.println("DATABASE_URL: " + env.getProperty("DATABASE_URL"));
    }
}