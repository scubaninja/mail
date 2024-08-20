using Xunit;
using System;
using System.Data;
using Dapper;
using Tailwind.Mail.Models;

public class BroadcastTests
{
    [Fact]
    public void FromMarkdownEmail_ValidMarkdownEmail_ReturnsBroadcast()
    {
        // Arrange
        var markdownEmail = new MarkdownEmail
        {
            Data = new
            {
                Subject = "Test Subject",
                Slug = "test-subject",
                SendToTag = "*"
            }
        };

        // Act
        var broadcast = Broadcast.FromMarkdownEmail(markdownEmail);

        // Assert
        Assert.NotNull(broadcast);
    }

    [Fact]
    public void FromMarkdownEmail_ValidMarkdownEmail_SetsName()
    {
        // Arrange
        var markdownEmail = new MarkdownEmail
        {
            Data = new
            {
                Subject = "Test Subject",
                Slug = "test-subject",
                SendToTag = "*"
            }
        };

        // Act
        var broadcast = Broadcast.FromMarkdownEmail(markdownEmail);

        // Assert
        Assert.Equal("Test Subject", broadcast.Name);
    }

    [Fact]
    public void FromMarkdownEmail_ValidMarkdownEmail_SetsSlug()
    {
        // Arrange
        var markdownEmail = new MarkdownEmail
        {
            Data = new
            {
                Subject = "Test Subject",
                Slug = "test-subject",
                SendToTag = "*"
            }
        };

        // Act
        var broadcast = Broadcast.FromMarkdownEmail(markdownEmail);

        // Assert
        Assert.Equal("test-subject", broadcast.Slug);
    }

    [Fact]
    public void FromMarkdownEmail_ValidMarkdownEmail_SetsSendToTag()
    {
        // Arrange
        var markdownEmail = new MarkdownEmail
        {
            Data = new
            {
                Subject = "Test Subject",
                Slug = "test-subject",
                SendToTag = "*"
            }
        };

        // Act
        var broadcast = Broadcast.FromMarkdownEmail(markdownEmail);

        // Assert
        Assert.Equal("*", broadcast.SendToTag);
    }

    [Fact]
    public void FromMarkdown_ValidMarkdown_ReturnsBroadcast()
    {
        // Arrange
        var markdown = @"
---
Subject: Test Subject
Slug: test-subject
SendToTag: *
---
";

        // Act
        var broadcast = Broadcast.FromMarkdown(markdown);

        // Assert
        Assert.NotNull(broadcast);
    }

    [Fact]
    public void FromMarkdown_ValidMarkdown_SetsName()
    {
        // Arrange
        var markdown = @"
---
Subject: Test Subject
Slug: test-subject
SendToTag: *
---
";

        // Act
        var broadcast = Broadcast.FromMarkdown(markdown);

        // Assert
        Assert.Equal("Test Subject", broadcast.Name);
    }

    [Fact]
    public void FromMarkdown_ValidMarkdown_SetsSlug()
    {
        // Arrange
        var markdown = @"
---
Subject: Test Subject
Slug: test-subject
SendToTag: *
---
";

        // Act
        var broadcast = Broadcast.FromMarkdown(markdown);

        // Assert
        Assert.Equal("test-subject", broadcast.Slug);
    }

    [Fact]
    public void FromMarkdown_ValidMarkdown_SetsSendToTag()
    {
        // Arrange
        var markdown = @"
---
Subject: Test Subject
Slug: test-subject
SendToTag: *
---
";

        // Act
        var broadcast = Broadcast.FromMarkdown(markdown);

        // Assert
        Assert.Equal("*", broadcast.SendToTag);
    }

    [Fact]
    public void ContactCount_ValidConnection_ReturnsContactCount()
    {
        // Arrange
        var broadcast = new Broadcast
        {
            SendToTag = "*"
        };

        var connection = new System.Data.SqlClient.SqlConnection("your_connection_string");

        // Act
        var contactCount = broadcast.ContactCount(connection);

        // Assert
        Assert.True(contactCount >= 0);
    }
}
